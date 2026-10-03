using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.DataAccess.Repository;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Business.Oneri;

/// <summary>
/// Öneri akışını yönetir ve sağlayıcının çıktısına GÜVENMEZ: yapay zeka listede olmayan bir kod
/// uydurabilir, aynı kodu iki kez yazabilir, can yeleğini unutabilir. Bu yüzden kodlar müsait
/// ekipman listesine göre süzülür, eksik can yelekleri tamamlanır, ücret ve depozito sağlayıcıdan
/// alınmaz, mevcut UcretHesapla ile burada hesaplanır. Yapılan her düzeltme sonuçta listelenir.
/// </summary>
public class OneriServisi(
    IMusaitlikServisi musaitlikServisi,
    IRepository<Musteri> musteriRepository,
    IRepository<Ekipman> ekipmanRepository,
    IAiOneriSaglayici saglayici,
    IYetkiServisi yetki) : IOneriServisi
{
    // Tek seferde ağırlanabilecek grup tavanı; sağlayıcının saçma bir tahmini (ör. 500) sınırlanır.
    public const int MaxKisiSayisi = 20;

    private const string VarsayilanAciklama = "Müşterinin ihtiyacına göre önerilen ekipman paketi.";
    private const string VarsayilanGuvenlikNotu = "Suya girmeden önce can yeleğinizi takıp tokalarını kapatın.";

    public async Task<OneriSonucu> OneriAlAsync(OneriIstegi istek, CancellationToken iptal = default)
    {
        yetki.YetkiKontrol(Islem.KiralamaIslemleri);
        if (string.IsNullOrWhiteSpace(istek.MusteriMetni))
            throw new DogrulamaException("Müşterinin ihtiyacını anlatan metni giriniz.");

        // Aralık kontrolü (bitiş > başlangıç) müsaitlik servisinde yapılır.
        var musaitler = musaitlikServisi.MusaitEkipmanlariGetir<Ekipman>(istek.Baslangic, istek.Bitis)
            .OrderBy(e => e.Kod)
            .ToList();
        var deneyim = MusteriDeneyimi(istek.MusteriId);
        var sure = istek.Bitis - istek.Baslangic;

        if (musaitler.Count == 0)
            return new OneriSonucu([], 0, "Seçilen aralıkta müsait ekipman yok.", VarsayilanGuvenlikNotu,
                0m, 0m, "Yok (sağlayıcı çağrılmadı)", null, EkipmanYeterli: false,
                ["Seçilen aralıkta müsait ekipman yok."]);

        var girdi = new OneriGirdisi(istek.MusteriMetni.Trim(), deneyim, sure,
            musaitler.Select(EkipmanOzeti.Olustur).ToList());
        var ham = await saglayici.OneriUretAsync(girdi, iptal);

        var duzeltmeler = new List<string>();
        var secilen = KodlariDogrula(ham.EkipmanKodlari, musaitler, duzeltmeler);

        var kisi = Math.Clamp(ham.KisiSayisi, 1, MaxKisiSayisi);
        if (kisi != ham.KisiSayisi)
            duzeltmeler.Add($"Tahmini kişi sayısı ({ham.KisiSayisi}) geçersiz olduğu için {kisi} kabul edildi.");

        var yeterli = CanYelekleriniTamamla(secilen, musaitler, kisi, duzeltmeler);
        yeterli &= TasimaKapasitesiYeterli(secilen, kisi, duzeltmeler);

        // Ücret modelden alınmaz: her ekipman kendi türüne göre hesaplar (polimorfizm).
        var ekipmanlar = secilen
            .Select(e => new OnerilenEkipman(e.Id, e.Kod, e.GetType().Name, e.UcretHesapla(sure), e.DepozitoTutari))
            .ToList();

        return new OneriSonucu(
            ekipmanlar,
            kisi,
            string.IsNullOrWhiteSpace(ham.Aciklama) ? VarsayilanAciklama : ham.Aciklama.Trim(),
            string.IsNullOrWhiteSpace(ham.GuvenlikNotu) ? VarsayilanGuvenlikNotu : ham.GuvenlikNotu.Trim(),
            ekipmanlar.Sum(e => e.TahminiUcret),
            ekipmanlar.Sum(e => e.Depozito),
            ham.Saglayici,
            ham.Uyari,
            yeterli,
            duzeltmeler);
    }

    private DeneyimSeviyesi? MusteriDeneyimi(int? musteriId)
    {
        if (musteriId is not int id)
            return null;

        var musteri = musteriRepository.GetById(id) ?? throw new KayitBulunamadiException("Müşteri", id);
        if (!musteri.AktifMi)
            throw new IslemYapilamazException($"{musteri.AdSoyad} pasif bir müşteri olduğu için öneri yapılamaz.");
        return musteri.DeneyimSeviyesi;
    }

    /// <summary>
    /// Sağlayıcının kodlarını sırasını koruyarak müsait ekipmanlara çevirir. Listede olmayan kod
    /// atılır; sebebi (hiç kayıtlı değil / o aralıkta müsait değil) tek bir sorguyla ayırt edilir.
    /// </summary>
    private List<Ekipman> KodlariDogrula(IReadOnlyList<string>? hamKodlar, List<Ekipman> musaitler, List<string> duzeltmeler)
    {
        var kodlar = (hamKodlar ?? [])
            .Select(k => (k ?? "").Trim().ToUpperInvariant())   // entity kodu büyük harfle tutar
            .Where(k => k.Length > 0)
            .ToList();
        var musaitKodlar = musaitler.ToDictionary(e => e.Kod);

        var listedeOlmayanlar = kodlar.Where(k => !musaitKodlar.ContainsKey(k)).Distinct().ToList();
        HashSet<string> kayitliOlanlar = listedeOlmayanlar.Count == 0
            ? []
            : ekipmanRepository.Query().Where(e => listedeOlmayanlar.Contains(e.Kod)).Select(e => e.Kod).ToHashSet();

        var secilen = new List<Ekipman>();
        var bildirilen = new HashSet<string>();
        foreach (var kod in kodlar)
        {
            if (musaitKodlar.TryGetValue(kod, out var ekipman))
            {
                if (secilen.Contains(ekipman))
                {
                    if (bildirilen.Add(kod))
                        duzeltmeler.Add($"{kod} birden fazla kez önerildiği için bir kez alındı.");
                    continue;
                }
                secilen.Add(ekipman);
            }
            else if (bildirilen.Add(kod))
            {
                duzeltmeler.Add(kayitliOlanlar.Contains(kod)
                    ? $"{kod} seçilen aralıkta müsait olmadığı için çıkarıldı."
                    : $"{kod} kayıtlı bir ekipman olmadığı için çıkarıldı.");
            }
        }
        return secilen;
    }

    /// <summary>
    /// Güvenlik kuralı: her kişiye bir can yeleği. Eksikse müsait yeleklerden (koda göre sıralı)
    /// tamamlanır. Yine de yetmiyorsa false döner.
    /// </summary>
    private static bool CanYelekleriniTamamla(List<Ekipman> secilen, List<Ekipman> musaitler, int kisi, List<string> duzeltmeler)
    {
        var yelekSayisi = secilen.Count(e => e is CanYelegi);
        if (yelekSayisi >= kisi)
            return true;

        var eklenecekler = musaitler.OfType<CanYelegi>()
            .Where(y => !secilen.Contains(y))
            .Take(kisi - yelekSayisi)
            .ToList();
        secilen.AddRange(eklenecekler);
        yelekSayisi += eklenecekler.Count;

        if (eklenecekler.Count > 0)
            duzeltmeler.Add($"{eklenecekler.Count} can yeleği eklendi (her kişiye bir can yeleği kuralı).");
        if (yelekSayisi >= kisi)
            return true;

        duzeltmeler.Add($"{kisi} kişi için yalnızca {yelekSayisi} müsait can yeleği var.");
        return false;
    }

    // SUP board bir kişi, kano kapasitesi kadar kişi taşır. Eksik tekne otomatik eklenmez, yalnızca bildirilir.
    private static bool TasimaKapasitesiYeterli(List<Ekipman> secilen, int kisi, List<string> duzeltmeler)
    {
        var kapasite = secilen.Sum(e => e switch
        {
            SupBoard => 1,
            Kano k => k.KisiKapasitesi,
            _ => 0
        });
        if (kapasite >= kisi)
            return true;

        duzeltmeler.Add(kapasite == 0
            ? $"Öneride SUP board veya kano yok; {kisi} kişi için ekipman yetersiz."
            : $"Önerilen ekipman {kapasite} kişi taşıyor, {kisi} kişi için yetersiz.");
        return false;
    }
}
