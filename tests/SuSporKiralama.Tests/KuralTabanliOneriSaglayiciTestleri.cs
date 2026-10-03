using SuSporKiralama.Business.Oneri;
using SuSporKiralama.Entities;
using SuSporKiralama.YapayZeka;

namespace SuSporKiralama.Tests;

public class KuralTabanliOneriSaglayiciTestleri
{
    // Veritabanı gerekmez: sağlayıcı yalnızca kendisine verilen özet listeyle çalışır.
    private static readonly EkipmanOzeti[] TumEkipmanlar =
    [
        new("SUP-001", "SupBoard", 150m, "saatlik", UzunlukCm: 320, MaxTasimaKg: 120, SupTipi: SupBoardTipi.Sisme),
        new("SUP-002", "SupBoard", 200m, "saatlik", UzunlukCm: 380, MaxTasimaKg: 110, SupTipi: SupBoardTipi.Sert),
        new("KANO-001", "Kano", 600m, "4 saatlik blok", KisiKapasitesi: 2),
        new("KANO-002", "Kano", 450m, "4 saatlik blok", KisiKapasitesi: 1),
        new("YLK-L1", "CanYelegi", 50m, "kiralama başı sabit", Beden: Beden.L),
        new("YLK-M1", "CanYelegi", 50m, "kiralama başı sabit", Beden: Beden.M),
        new("YLK-M2", "CanYelegi", 50m, "kiralama başı sabit", Beden: Beden.M),
        new("YLK-S1", "CanYelegi", 50m, "kiralama başı sabit", Beden: Beden.S),
    ];

    private readonly KuralTabanliOneriSaglayici _saglayici = new();

    private Task<HamOneri> Oner(string metin, DeneyimSeviyesi? deneyim = null, IReadOnlyList<EkipmanOzeti>? liste = null) =>
        _saglayici.OneriUretAsync(new OneriGirdisi(metin, deneyim, TimeSpan.FromHours(3), liste ?? TumEkipmanlar));

    [Fact]
    public async Task IlkKezSakinIkiKisi_KanoVeIkiYelek()
    {
        var oneri = await Oner("2 kişiyiz, ilk kez deneyeceğiz, sakin bir şey istiyoruz");

        Assert.Equal(2, oneri.KisiSayisi);
        Assert.Equal(["KANO-001", "YLK-M1", "YLK-M2"], oneri.EkipmanKodlari);
        Assert.Contains("Kıyıya yakın", oneri.GuvenlikNotu);
        Assert.Equal(KuralTabanliOneriSaglayici.Ad, oneri.Saglayici);
        Assert.Null(oneri.Uyari);
    }

    [Fact]
    public async Task TekBasinaSupDeneyimli_SertSup()
    {
        var oneri = await Oner("Tek başıma SUP yapmak istiyorum", DeneyimSeviyesi.Ileri);

        Assert.Equal(1, oneri.KisiSayisi);
        Assert.Equal(["SUP-002", "YLK-M1"], oneri.EkipmanKodlari);
    }

    [Fact]
    public async Task YetiskinVeCocuk_SayilarToplanir_KucukBedenOnce()
    {
        var oneri = await Oner("2 yetişkin 1 çocuk, kano istiyoruz");

        Assert.Equal(3, oneri.KisiSayisi);
        Assert.Equal(["KANO-001", "KANO-002", "YLK-S1", "YLK-M1", "YLK-M2"], oneri.EkipmanKodlari);
        Assert.Contains("Çocuklar yetişkin gözetiminde", oneri.GuvenlikNotu);
    }

    [Fact]
    public async Task IpucuYok_BirKisiSismeSup()
    {
        // "bir şeyler" kişi sayısı değildir.
        var oneri = await Oner("Merhaba, bir şeyler kiralamak istiyoruz");

        Assert.Equal(1, oneri.KisiSayisi);
        Assert.Equal(["SUP-001", "YLK-M1"], oneri.EkipmanKodlari);
    }

    [Fact]
    public async Task TurkceBuyukHarf_Taninir()
    {
        var oneri = await Oner("İKİ KİŞİYİZ, İLK KEZ BİNECEĞİZ");

        Assert.Equal(2, oneri.KisiSayisi);
        Assert.Equal("KANO-001", oneri.EkipmanKodlari[0]);
    }

    [Fact]
    public async Task BaslangicDeneyimi_IpucuOlmasaBileKanoTercihEdilir()
    {
        var oneri = await Oner("3 kişiyiz", DeneyimSeviyesi.Baslangic);

        Assert.Equal(["KANO-001", "KANO-002"], oneri.EkipmanKodlari.Where(k => k.StartsWith("KANO")));
        Assert.DoesNotContain(oneri.EkipmanKodlari, k => k.StartsWith("SUP"));
    }

    [Fact]
    public async Task KanoYoksa_SupIleTamamlanir()
    {
        var kanosuz = TumEkipmanlar.Where(e => e.KisiKapasitesi is null).ToList();

        var oneri = await Oner("ikimiz de ilk kez deneyeceğiz", liste: kanosuz);

        Assert.Equal(2, oneri.KisiSayisi);
        Assert.Equal(["SUP-001", "SUP-002", "YLK-M1", "YLK-M2"], oneri.EkipmanKodlari);
    }

    [Fact]
    public async Task AyniGirdi_AyniOneri()
    {
        var a = await Oner("2 yetişkin 1 çocuk, sakin");
        var b = await Oner("2 yetişkin 1 çocuk, sakin");

        Assert.Equal(a.EkipmanKodlari, b.EkipmanKodlari);
        Assert.Equal((a.KisiSayisi, a.Aciklama, a.GuvenlikNotu), (b.KisiSayisi, b.Aciklama, b.GuvenlikNotu));
    }

    [Fact]
    public async Task IptalEdilmisIstek_OperationCanceled()
    {
        var girdi = new OneriGirdisi("tek kişi", null, TimeSpan.FromHours(1), TumEkipmanlar);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            _saglayici.OneriUretAsync(girdi, new CancellationToken(canceled: true)));
    }
}
