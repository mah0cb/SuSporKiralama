using Microsoft.Extensions.Time.Testing;
using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Business.Oneri;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Tests;

/// <summary>Testte istenen ham öneriyi döndüren, kendisine gelen girdiyi kaydeden sahte sağlayıcı.</summary>
public class SabitOneriSaglayici(HamOneri oneri) : IAiOneriSaglayici
{
    public List<OneriGirdisi> Girdiler { get; } = [];

    public Task<HamOneri> OneriUretAsync(OneriGirdisi girdi, CancellationToken iptal = default)
    {
        Girdiler.Add(girdi);
        return Task.FromResult(oneri);
    }
}

public class OneriServisiTestleri : IDisposable
{
    // "Şimdi" 1 Temmuz 10:00; öneri aralığı 14:00–19:00 (5 saat).
    // SUP 150 TL/saat → 750 TL; 2 kişilik kano 600 TL/4 saatlik blok → 2 blok = 1200 TL; yelek 50 TL sabit.
    private static readonly DateTime Bugun = new(2026, 7, 1);
    private static DateTime Saat(int saat) => Bugun.AddHours(saat);

    private readonly TestVeritabani _db = new();
    private readonly FakeTimeProvider _zaman = new(new DateTimeOffset(Saat(10), TimeSpan.Zero));
    private readonly Musteri _musteri;
    private readonly SupBoard _sup;
    private readonly Kano _kano;

    public OneriServisiTestleri()
    {
        _musteri = _db.MusteriEkle();
        _sup = _db.SupEkle("SUP-001");
        _kano = IkiKisilikKano("KANO-001");
        _db.YelekEkle("YLK-001");
        _db.YelekEkle("YLK-002");
    }

    public void Dispose() => _db.Dispose();

    private Kano IkiKisilikKano(string kod)
    {
        var kano = new Kano(kod, "Pelican", "Argo 2", 600m, 2500m, 2);
        _db.Context.Ekipmanlar.Add(kano);
        _db.Context.SaveChanges();
        return kano;
    }

    private static HamOneri Ham(int kisi, params string[] kodlar) =>
        new(kodlar, kisi, "Test açıklaması", "Test güvenlik notu", "Sahte");

    private OneriServisi Servis(IAiOneriSaglayici saglayici, Oturum? oturum = null)
    {
        oturum ??= TestOturumu.PersonelOlarakGiris();
        var musaitlik = new MusaitlikServisi(_db.Repo<Ekipman>(), _db.Repo<KiralamaDetay>(), _zaman);
        return new OneriServisi(musaitlik, _db.Repo<Musteri>(), _db.Repo<Ekipman>(), saglayici, oturum.Yetki());
    }

    private Task<OneriSonucu> Oner(HamOneri ham, int? musteriId = null) =>
        Servis(new SabitOneriSaglayici(ham)).OneriAlAsync(new OneriIstegi("2 kişiyiz", Saat(14), Saat(19), musteriId));

    private static List<string> Kodlar(OneriSonucu sonuc) => sonuc.Ekipmanlar.Select(e => e.Kod).ToList();

    // --- Yetki ve girdi ---

    [Fact]
    public async Task OturumYok_YetkisizIslem_SaglayiciCagrilmaz()
    {
        var saglayici = new SabitOneriSaglayici(Ham(1, "SUP-001"));
        var servis = Servis(saglayici, new Oturum());

        await Assert.ThrowsAsync<YetkisizIslemException>(() =>
            servis.OneriAlAsync(new OneriIstegi("tek kişi", Saat(14), Saat(19))));
        Assert.Empty(saglayici.Girdiler);
    }

    [Fact]
    public async Task AdminVePersonel_OneriAlabilir()
    {
        var istek = new OneriIstegi("tek kişi", Saat(14), Saat(19));
        var ham = Ham(1, "SUP-001", "YLK-001");

        Assert.NotEmpty((await Servis(new SabitOneriSaglayici(ham), TestOturumu.AdminOlarakGiris()).OneriAlAsync(istek)).Ekipmanlar);
        Assert.NotEmpty((await Servis(new SabitOneriSaglayici(ham), TestOturumu.PersonelOlarakGiris()).OneriAlAsync(istek)).Ekipmanlar);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task BosMetin_DogrulamaException(string metin)
    {
        await Assert.ThrowsAsync<DogrulamaException>(() =>
            Servis(new SabitOneriSaglayici(Ham(1))).OneriAlAsync(new OneriIstegi(metin, Saat(14), Saat(19))));
    }

    [Fact]
    public async Task BitisBaslangictanOnce_DogrulamaException()
    {
        await Assert.ThrowsAsync<DogrulamaException>(() =>
            Servis(new SabitOneriSaglayici(Ham(1))).OneriAlAsync(new OneriIstegi("tek kişi", Saat(19), Saat(14))));
    }

    [Fact]
    public async Task OlmayanMusteri_KayitBulunamadi()
    {
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => Oner(Ham(1), musteriId: 999));
    }

    [Fact]
    public async Task PasifMusteri_IslemYapilamaz()
    {
        _musteri.AktifMi = false;
        _db.Context.SaveChanges();

        await Assert.ThrowsAsync<IslemYapilamazException>(() => Oner(Ham(1), _musteri.Id));
    }

    [Fact]
    public async Task SaglayiciyaDeneyimSureVeYalnizcaMusaitEkipmanlarVerilir()
    {
        _musteri.DeneyimSeviyesi = DeneyimSeviyesi.Baslangic;
        _db.Context.SaveChanges();
        var dolu = _db.SupEkle("SUP-002");
        _db.KiralamaKur(_musteri, _db.PersonelEkle(), Saat(13), Saat(15), KiralamaDurumu.Rezerve, dolu);
        var saglayici = new SabitOneriSaglayici(Ham(1, "SUP-001"));

        await Servis(saglayici).OneriAlAsync(new OneriIstegi("  sakin bir şey  ", Saat(14), Saat(19), _musteri.Id));

        var girdi = Assert.Single(saglayici.Girdiler);
        Assert.Equal("sakin bir şey", girdi.MusteriMetni);
        Assert.Equal(DeneyimSeviyesi.Baslangic, girdi.Deneyim);
        Assert.Equal(TimeSpan.FromHours(5), girdi.Sure);
        Assert.Equal(["KANO-001", "SUP-001", "YLK-001", "YLK-002"], girdi.MusaitEkipmanlar.Select(e => e.Kod));

        var sup = girdi.MusaitEkipmanlar.Single(e => e.Kod == "SUP-001");
        Assert.Equal((320, 120, SupBoardTipi.Sisme), (sup.UzunlukCm!.Value, sup.MaxTasimaKg!.Value, sup.SupTipi!.Value));
        Assert.Equal(2, girdi.MusaitEkipmanlar.Single(e => e.Kod == "KANO-001").KisiKapasitesi);
        Assert.Equal(Beden.M, girdi.MusaitEkipmanlar.Single(e => e.Kod == "YLK-001").Beden);
    }

    [Fact]
    public async Task MusteriSecilmezse_DeneyimNull()
    {
        var saglayici = new SabitOneriSaglayici(Ham(1, "SUP-001"));
        await Servis(saglayici).OneriAlAsync(new OneriIstegi("tek kişi", Saat(14), Saat(19)));

        Assert.Null(Assert.Single(saglayici.Girdiler).Deneyim);
    }

    // --- Kodların doğrulanması ---

    [Fact]
    public async Task ListedeOlmayanKodlar_AtilirVeSebebiYazilir()
    {
        var dolu = _db.SupEkle("SUP-009");
        _db.KiralamaKur(_musteri, _db.PersonelEkle(), Saat(13), Saat(15), KiralamaDurumu.Rezerve, dolu);

        var sonuc = await Oner(Ham(1, "SUP-009", "SUP-001", "XYZ-123", "YLK-001"));

        Assert.Equal(["SUP-001", "YLK-001"], Kodlar(sonuc));
        Assert.Contains("SUP-009 seçilen aralıkta müsait olmadığı için çıkarıldı.", sonuc.Duzeltmeler);
        Assert.Contains("XYZ-123 kayıtlı bir ekipman olmadığı için çıkarıldı.", sonuc.Duzeltmeler);
        Assert.True(sonuc.EkipmanYeterli);
    }

    [Fact]
    public async Task BakimdakiEkipman_MusaitDegilSayilir()
    {
        _sup.Durum = EkipmanDurumu.Bakimda;
        _db.Context.SaveChanges();

        var sonuc = await Oner(Ham(1, "SUP-001", "KANO-001", "YLK-001"));

        Assert.Equal(["KANO-001", "YLK-001"], Kodlar(sonuc));
        Assert.Contains("SUP-001 seçilen aralıkta müsait olmadığı için çıkarıldı.", sonuc.Duzeltmeler);
    }

    [Fact]
    public async Task TekrarEdenVeKucukHarfliKodlar_BirKezAlinir()
    {
        var sonuc = await Oner(Ham(1, "sup-001", " SUP-001 ", "SUP-001", "ylk-001", ""));

        Assert.Equal(["SUP-001", "YLK-001"], Kodlar(sonuc));
        Assert.Equal(["SUP-001 birden fazla kez önerildiği için bir kez alındı."], sonuc.Duzeltmeler);
    }

    // --- Güvenlik kuralı ve yeterlilik ---

    [Fact]
    public async Task EksikCanYelekleri_MusaitlerdenTamamlanir()
    {
        var sonuc = await Oner(Ham(2, "KANO-001"));

        Assert.Equal(["KANO-001", "YLK-001", "YLK-002"], Kodlar(sonuc));
        Assert.Contains("2 can yeleği eklendi (her kişiye bir can yeleği kuralı).", sonuc.Duzeltmeler);
        Assert.True(sonuc.EkipmanYeterli);
    }

    [Fact]
    public async Task CanYelegiYetersiz_EkipmanYeterliFalse()
    {
        IkiKisilikKano("KANO-002");

        var sonuc = await Oner(Ham(3, "KANO-001", "KANO-002", "YLK-001"));

        Assert.False(sonuc.EkipmanYeterli);
        Assert.Contains("1 can yeleği eklendi (her kişiye bir can yeleği kuralı).", sonuc.Duzeltmeler);
        Assert.Contains("3 kişi için yalnızca 2 müsait can yeleği var.", sonuc.Duzeltmeler);
    }

    [Fact]
    public async Task TasimaKapasitesiYetersiz_EkipmanYeterliFalse_TekneEklenmez()
    {
        var sonuc = await Oner(Ham(2, "SUP-001", "YLK-001", "YLK-002"));

        Assert.False(sonuc.EkipmanYeterli);
        Assert.Equal(["SUP-001", "YLK-001", "YLK-002"], Kodlar(sonuc));
        Assert.Contains("Önerilen ekipman 1 kişi taşıyor, 2 kişi için yetersiz.", sonuc.Duzeltmeler);
    }

    [Fact]
    public async Task YalnizcaYelekOnerilirse_EkipmanYetersiz()
    {
        var sonuc = await Oner(Ham(1, "YLK-001"));

        Assert.False(sonuc.EkipmanYeterli);
        Assert.Contains("Öneride SUP board veya kano yok; 1 kişi için ekipman yetersiz.", sonuc.Duzeltmeler);
    }

    [Fact]
    public async Task HicMusaitEkipmanYok_SaglayiciCagrilmaz()
    {
        foreach (var ekipman in _db.Context.Ekipmanlar)
            ekipman.Durum = EkipmanDurumu.HizmetDisi;
        _db.Context.SaveChanges();
        var saglayici = new SabitOneriSaglayici(Ham(1, "SUP-001"));

        var sonuc = await Servis(saglayici).OneriAlAsync(new OneriIstegi("tek kişi", Saat(14), Saat(19)));

        Assert.Empty(saglayici.Girdiler);
        Assert.Empty(sonuc.Ekipmanlar);
        Assert.False(sonuc.EkipmanYeterli);
        Assert.Equal(["Seçilen aralıkta müsait ekipman yok."], sonuc.Duzeltmeler);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-3, 1)]
    [InlineData(500, OneriServisi.MaxKisiSayisi)]
    public async Task GecersizKisiSayisi_SinirlaraCekilir(int ham, int beklenen)
    {
        var sonuc = await Oner(Ham(ham, "SUP-001"));

        Assert.Equal(beklenen, sonuc.KisiSayisi);
        Assert.Contains($"Tahmini kişi sayısı ({ham}) geçersiz olduğu için {beklenen} kabul edildi.", sonuc.Duzeltmeler);
    }

    // --- Ücret ---

    [Fact]
    public async Task UcretVeDepozito_UcretHesaplaIleHesaplanir()
    {
        var sonuc = await Oner(Ham(2, "SUP-001", "KANO-001", "YLK-001", "YLK-002"));

        Assert.Equal(750m, sonuc.Ekipmanlar.Single(e => e.Kod == "SUP-001").TahminiUcret);   // 5 saat × 150
        Assert.Equal(1200m, sonuc.Ekipmanlar.Single(e => e.Kod == "KANO-001").TahminiUcret); // 2 blok × 600
        Assert.Equal(50m, sonuc.Ekipmanlar.Single(e => e.Kod == "YLK-001").TahminiUcret);    // sabit
        Assert.Equal(2050m, sonuc.TahminiToplamUcret);
        Assert.Equal(1500m + 2500m + 200m + 200m, sonuc.ToplamDepozito);
        Assert.Equal(_kano.Id, sonuc.Ekipmanlar.Single(e => e.Kod == "KANO-001").Id);
        Assert.Equal(nameof(Kano), sonuc.Ekipmanlar.Single(e => e.Kod == "KANO-001").Tur);
        Assert.Empty(sonuc.Duzeltmeler);
    }

    [Fact]
    public async Task SaglayiciAdiVeUyarisiTasinir_BosMetinlereVarsayilanVerilir()
    {
        var ham = new HamOneri(["SUP-001", "YLK-001"], 1, " ", "", "Kural tabanlı", "Yapay zeka kullanılamıyor");

        var sonuc = await Oner(ham);

        Assert.Equal("Kural tabanlı", sonuc.Saglayici);
        Assert.Equal("Yapay zeka kullanılamıyor", sonuc.SaglayiciUyarisi);
        Assert.False(string.IsNullOrWhiteSpace(sonuc.Aciklama));
        Assert.False(string.IsNullOrWhiteSpace(sonuc.GuvenlikNotu));
    }
}
