using System.Globalization;
using System.Text.RegularExpressions;
using SuSporKiralama.Business.Oneri;
using SuSporKiralama.Entities;

namespace SuSporKiralama.YapayZeka;

/// <summary>
/// İnternet ve API anahtarı olmadan çalışan, deterministik yedek sağlayıcı. Metindeki basit
/// ipuçlarından (sayılar, "ilk kez", "sakin", "kano", "çocuk" gibi kelimeler) ve müşterinin deneyim
/// seviyesinden kişi sayısını ve ekipmanı seçer. Aynı girdi her zaman aynı öneriyi verir.
/// </summary>
public class KuralTabanliOneriSaglayici : IAiOneriSaglayici
{
    public const string Ad = "Kural tabanlı";

    private static readonly CultureInfo Turkce = CultureInfo.GetCultureInfo("tr-TR");

    // "2 kişiyiz", "iki yetişkin", "1 çocuk" gibi ifadeler; bulunan sayılar toplanır.
    private static readonly Regex KisiIfadesi = new(
        @"\b(\d+|bir|iki|üç|dört|beş|altı|yedi|sekiz|dokuz|on)\s*(kişi|yetişkin|büyük|çocuk)",
        RegexOptions.CultureInvariant);

    private static readonly Dictionary<string, int> SayiKelimeleri = new()
    {
        ["bir"] = 1, ["iki"] = 2, ["üç"] = 3, ["dört"] = 4, ["beş"] = 5,
        ["altı"] = 6, ["yedi"] = 7, ["sekiz"] = 8, ["dokuz"] = 9, ["on"] = 10
    };

    private static readonly string[] SakinIpuclari = ["ilk kez", "ilk defa", "sakin", "acemi", "yeni başla", "hiç denemedik", "hiç binmedik"];
    private static readonly string[] IleriIpuclari = ["deneyimli", "hızlı", "uzun mesafe"];
    private static readonly string[] IkiKisiIpuclari = ["ikimiz", "eşimle", "sevgilimle", "arkadaşımla"];

    public Task<HamOneri> OneriUretAsync(OneriGirdisi girdi, CancellationToken iptal = default)
    {
        iptal.ThrowIfCancellationRequested();
        return Task.FromResult(Oner(girdi));
    }

    private static HamOneri Oner(OneriGirdisi girdi)
    {
        // Türkçe küçük harf: "İLK KEZ" → "ilk kez" (sabit kültürde "i̇lk" olurdu).
        var metin = girdi.MusteriMetni.ToLower(Turkce);
        var kisi = KisiSayisi(metin);
        var cocukVar = metin.Contains("çocuk");
        var sakin = cocukVar || girdi.Deneyim == DeneyimSeviyesi.Baslangic || IcerirMi(metin, SakinIpuclari);
        var ileri = !sakin && (girdi.Deneyim == DeneyimSeviyesi.Ileri || IcerirMi(metin, IleriIpuclari));

        var kanoIstendi = metin.Contains("kano");
        var supIstendi = metin.Contains("sup") || metin.Contains("board");
        // Kano iki kişiyi tek teknede taşır ve devrilmesi zordur; kalabalık ve sakin gruplara uygundur.
        var kanoOncelikli = kanoIstendi || (!supIstendi && sakin && kisi >= 2);

        var liste = girdi.MusaitEkipmanlar;
        var kanolar = liste.Where(e => e.KisiKapasitesi is not null)
            .OrderByDescending(e => e.KisiKapasitesi).ThenBy(e => e.Kod);
        var tercihEdilenTip = ileri ? SupBoardTipi.Sert : SupBoardTipi.Sisme;
        var suplar = liste.Where(e => e.SupTipi is not null)
            .OrderBy(e => e.SupTipi == tercihEdilenTip ? 0 : 1).ThenBy(e => e.Kod);

        // Tercih edilen türle başlanır, yetmezse diğer türle tamamlanır.
        var tekneler = new List<EkipmanOzeti>();
        var kalan = kisi;
        foreach (var aday in kanoOncelikli ? kanolar.Concat(suplar) : suplar.Concat(kanolar))
        {
            if (kalan <= 0) break;
            tekneler.Add(aday);
            kalan -= aday.KisiKapasitesi ?? 1;
        }

        // Her kişiye bir yelek; çocuk varsa küçük bedenler önce, yoksa M'ye en yakın beden.
        var yelekler = liste.Where(e => e.Beden is not null)
            .OrderBy(e => cocukVar ? (int)e.Beden!.Value : Math.Abs((int)e.Beden!.Value - (int)Beden.M))
            .ThenBy(e => e.Kod)
            .Take(kisi);

        var kodlar = tekneler.Concat(yelekler).Select(e => e.Kod).ToList();
        return new HamOneri(kodlar, kisi, Aciklama(kisi, tekneler), GuvenlikNotu(sakin, cocukVar), Ad);
    }

    private static int KisiSayisi(string metin)
    {
        var toplam = KisiIfadesi.Matches(metin)
            .Sum(m => int.TryParse(m.Groups[1].Value, out var sayi) ? sayi : SayiKelimeleri[m.Groups[1].Value]);
        if (toplam > 0)
            return Math.Min(toplam, OneriServisi.MaxKisiSayisi);
        return IcerirMi(metin, IkiKisiIpuclari) ? 2 : 1;
    }

    private static bool IcerirMi(string metin, string[] ipuclari) => ipuclari.Any(metin.Contains);

    private static string Aciklama(int kisi, List<EkipmanOzeti> tekneler)
    {
        if (tekneler.Count == 0)
            return $"{kisi} kişi için müsait SUP board veya kano bulunamadı.";
        if (tekneler.All(e => e.KisiKapasitesi is not null))
            return $"{kisi} kişi için kano önerildi: kanolar dengelidir, sakin bir gezinti ve yeni başlayanlar için uygundur.";
        if (tekneler.All(e => e.SupTipi == SupBoardTipi.Sert))
            return $"{kisi} kişi için sert SUP board önerildi: daha hızlı ve deneyimli kullanıcılar için uygundur.";
        return $"{kisi} kişi için SUP board ağırlıklı paket önerildi: şişme SUP dengelidir, başlangıç için uygundur.";
    }

    private static string GuvenlikNotu(bool sakin, bool cocukVar)
    {
        var not = "Herkes suya girmeden önce can yeleğini takıp tokalarını kapatmalıdır.";
        if (sakin)
            not += " Kıyıya yakın kalın ve rüzgârlı havada açığa çıkmayın.";
        if (cocukVar)
            not += " Çocuklar yetişkin gözetiminde olmalı ve kendi bedenine uygun yelek giymelidir.";
        return not;
    }
}
