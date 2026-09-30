using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Tests;

public class EkipmanServisiTestleri : IDisposable
{
    private readonly TestVeritabani _db = new();
    private readonly EkipmanServisi _servis;

    public EkipmanServisiTestleri()
    {
        _servis = new EkipmanServisi(_db.Repo<Ekipman>(), _db.Repo<KiralamaDetay>());
    }

    public void Dispose() => _db.Dispose();

    private static SupBoard YeniSup(string kod = "SUP-001", decimal ucret = 150m) =>
        new(kod, "Aqua Marina", "Beast", ucret, 1500m, 320, 120, SupBoardTipi.Sisme);

    // Filtreleme testleri için karışık envanter.
    private void EnvanterEkle()
    {
        _servis.Ekle(YeniSup("SUP-001"));
        _servis.Ekle(new SupBoard("SUP-002", "Starboard", "Go", 250m, 3000m, 340, 130, SupBoardTipi.Sert) { Durum = EkipmanDurumu.Bakimda });
        _servis.Ekle(new Kano("KANO-001", "Pelican", "Argo", 600m, 2500m, 1));
        _servis.Ekle(new Kano("KANO-002", "Old Town", "Guide", 900m, 4500m, 3) { Durum = EkipmanDurumu.HizmetDisi });
        _servis.Ekle(new CanYelegi("YLK-001", "Decathlon", "Itiwit", 50m, 200m, Beden.M));
    }

    // --- Ekleme ---

    [Fact]
    public void Ekle_Gecerli_Basarili()
    {
        var sup = YeniSup();

        _servis.Ekle(sup);

        Assert.IsType<SupBoard>(_servis.IdIleGetir(sup.Id));
    }

    [Fact]
    public void Ekle_AyniKodKucukHarf_BenzersizlikIhlali()
    {
        _servis.Ekle(YeniSup("SUP-001"));

        var hata = Assert.Throws<BenzersizlikIhlaliException>(() => _servis.Ekle(YeniSup("sup-001")));
        Assert.Equal("SUP-001", hata.Deger);
    }

    [Fact]
    public void Ekle_SifirUcret_DogrulamaException()
    {
        Assert.Throws<DogrulamaException>(() => _servis.Ekle(YeniSup(ucret: 0m)));
    }

    [Fact]
    public void Ekle_KiradaDurumunda_IslemYapilamaz()
    {
        var sup = YeniSup();
        sup.Durum = EkipmanDurumu.Kirada;

        Assert.Throws<IslemYapilamazException>(() => _servis.Ekle(sup));
    }

    // --- Filtreleme ---

    [Fact]
    public void Filtrele_TureGore_SadeceOTurDoner()
    {
        EnvanterEkle();

        var kanolar = _servis.Filtrele<Kano>();

        Assert.Equal(2, kanolar.Count);
        Assert.All(kanolar, k => Assert.StartsWith("KANO", k.Kod));
    }

    [Fact]
    public void Filtrele_TurVeDurum_BirlikteUygulanir()
    {
        EnvanterEkle();

        var sonuc = Assert.Single(_servis.Filtrele<SupBoard>(EkipmanDurumu.Musait));
        Assert.Equal("SUP-001", sonuc.Kod);
    }

    [Fact]
    public void Filtrele_EkipmanIle_SadeceDurumaGore()
    {
        EnvanterEkle();

        var sonuc = Assert.Single(_servis.Filtrele<Ekipman>(EkipmanDurumu.HizmetDisi));
        Assert.Equal("KANO-002", sonuc.Kod);
        Assert.Equal(5, _servis.Filtrele<Ekipman>().Count);
    }

    [Fact]
    public void MusaitleriGetir_SadeceMusaitDurumdakiler()
    {
        EnvanterEkle();

        var musaitler = _servis.MusaitleriGetir();

        Assert.Equal(["KANO-001", "SUP-001", "YLK-001"], musaitler.Select(e => e.Kod).Order());
    }

    // --- Fiyat ---

    [Fact]
    public void FiyatGuncelle_Pozitif_Guncellenir()
    {
        var sup = YeniSup();
        _servis.Ekle(sup);

        _servis.FiyatGuncelle(sup.Id, 199.90m);

        Assert.Equal(199.90m, _servis.IdIleGetir(sup.Id).BirimUcret);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void FiyatGuncelle_SifirVeyaNegatif_DogrulamaException(decimal fiyat)
    {
        var sup = YeniSup();
        _servis.Ekle(sup);

        Assert.Throws<DogrulamaException>(() => _servis.FiyatGuncelle(sup.Id, fiyat));
        Assert.Equal(150m, sup.BirimUcret);
    }

    [Fact]
    public void FiyatGuncelle_OlmayanId_KayitBulunamadi()
    {
        Assert.Throws<KayitBulunamadiException>(() => _servis.FiyatGuncelle(99, 100m));
    }

    // --- Durum ---

    [Fact]
    public void DurumDegistir_BakimaAl_Basarili()
    {
        var sup = YeniSup();
        _servis.Ekle(sup);

        _servis.DurumDegistir(sup.Id, EkipmanDurumu.Bakimda);

        Assert.Equal(EkipmanDurumu.Bakimda, _servis.IdIleGetir(sup.Id).Durum);
    }

    [Fact]
    public void DurumDegistir_KiradaYap_IslemYapilamaz()
    {
        var sup = YeniSup();
        _servis.Ekle(sup);

        Assert.Throws<IslemYapilamazException>(() => _servis.DurumDegistir(sup.Id, EkipmanDurumu.Kirada));
        Assert.Equal(EkipmanDurumu.Musait, sup.Durum);
    }

    [Fact]
    public void DurumDegistir_KiradakiEkipman_IslemYapilamaz()
    {
        // Kiradaki ekipman kiralama akışıyla oluşur; burada doğrudan veritabanına yazılır.
        var sup = _db.SupEkle(durum: EkipmanDurumu.Kirada);
        _db.KiralamaEkle(_db.MusteriEkle(), _db.PersonelEkle(), sup, aktif: true);

        Assert.Throws<IslemYapilamazException>(() => _servis.DurumDegistir(sup.Id, EkipmanDurumu.Musait));
        Assert.Equal(EkipmanDurumu.Kirada, sup.Durum);
    }

    [Fact]
    public void Guncelle_DurumuKiradaYap_IslemYapilamaz()
    {
        var sup = YeniSup();
        _servis.Ekle(sup);

        sup.Durum = EkipmanDurumu.Kirada;

        Assert.Throws<IslemYapilamazException>(() => _servis.Guncelle(sup));
    }

    [Fact]
    public void Guncelle_KiradakiEkipmanDigerAlan_Basarili()
    {
        var sup = _db.SupEkle(durum: EkipmanDurumu.Kirada);

        sup.Marka = "Red Paddle";
        _servis.Guncelle(sup);

        Assert.Equal("Red Paddle", _servis.IdIleGetir(sup.Id).Marka);
    }

    // --- Silme ---

    [Fact]
    public void Sil_GecmisiYok_Silinir()
    {
        var sup = YeniSup();
        _servis.Ekle(sup);

        _servis.Sil(sup.Id);

        Assert.Empty(_servis.TumunuGetir());
    }

    [Fact]
    public void Sil_KiralamaGecmisiVar_IliskiliKayitVar_HizmetDisiOnerilir()
    {
        var sup = _db.SupEkle();
        _db.KiralamaEkle(_db.MusteriEkle(), _db.PersonelEkle(), sup);

        var hata = Assert.Throws<IliskiliKayitVarException>(() => _servis.Sil(sup.Id));
        Assert.Contains("Hizmet Dışı", hata.Message);
    }
}
