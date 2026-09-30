using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Tests;

public class MusteriServisiTestleri : IDisposable
{
    private readonly TestVeritabani _db = new();
    private readonly MusteriServisi _servis;

    public MusteriServisiTestleri()
    {
        _servis = new MusteriServisi(_db.Repo<Musteri>(), _db.Repo<Kiralama>());
    }

    public void Dispose() => _db.Dispose();

    private static Musteri YeniMusteri(string telefon, string ad = "Ayşe", string soyad = "Yılmaz") =>
        new(ad, soyad, telefon, DeneyimSeviyesi.Baslangic);

    // --- Ekleme ve telefon ---

    [Theory]
    [InlineData("0532 111 22 33")]
    [InlineData("+90 532 111 22 33")]
    [InlineData("5321112233")]
    [InlineData("(0532) 111-22-33")]
    public void Ekle_FarkliTelefonBicimleri_NormalizeEdilir(string telefon)
    {
        var musteri = YeniMusteri(telefon);

        _servis.Ekle(musteri);

        Assert.True(musteri.Id > 0);
        Assert.Equal("05321112233", _servis.IdIleGetir(musteri.Id).Telefon);
    }

    [Theory]
    [InlineData("02121112233")]   // sabit hat
    [InlineData("0532111223")]    // eksik hane
    [InlineData("0532ABC2233")]   // harf
    public void Ekle_GecersizTelefon_DogrulamaException(string telefon)
    {
        Assert.Throws<DogrulamaException>(() => _servis.Ekle(YeniMusteri(telefon)));
        Assert.Empty(_servis.TumunuGetir());
    }

    [Fact]
    public void Ekle_AyniTelefonFarkliBicim_BenzersizlikIhlali()
    {
        _servis.Ekle(YeniMusteri("05321112233"));

        var hata = Assert.Throws<BenzersizlikIhlaliException>(() => _servis.Ekle(YeniMusteri("+90 532 111 22 33", "Mehmet", "Demir")));
        Assert.Equal("Telefon", hata.AlanAdi);
    }

    // --- E-posta ---

    [Fact]
    public void Ekle_GecersizEposta_DogrulamaException()
    {
        var musteri = YeniMusteri("05321112233");
        musteri.Eposta = "ali@x"; // entity kabul eder, servis alan adında nokta ister

        Assert.Throws<DogrulamaException>(() => _servis.Ekle(musteri));
    }

    [Fact]
    public void Ekle_EpostasizVeGecerliEpostali_Basarili()
    {
        _servis.Ekle(YeniMusteri("05321112233"));
        _servis.Ekle(new Musteri("Elif", "Şahin", "05343334455", DeneyimSeviyesi.Ileri) { Eposta = "elif@example.com" });

        Assert.Equal(2, _servis.TumunuGetir().Count);
    }

    // --- Güncelleme ---

    [Fact]
    public void Guncelle_KendiTelefonuyla_Basarili()
    {
        var musteri = YeniMusteri("05321112233");
        _servis.Ekle(musteri);

        musteri.Ad = "Ayşegül";
        _servis.Guncelle(musteri);

        Assert.Equal("Ayşegül", _servis.IdIleGetir(musteri.Id).Ad);
    }

    [Fact]
    public void Guncelle_BaskasininTelefonu_BenzersizlikIhlali()
    {
        _servis.Ekle(YeniMusteri("05321112233"));
        var ikinci = YeniMusteri("05332223344", "Mehmet", "Demir");
        _servis.Ekle(ikinci);

        ikinci.Telefon = "0532 111 22 33";

        Assert.Throws<BenzersizlikIhlaliException>(() => _servis.Guncelle(ikinci));
    }

    [Fact]
    public void Guncelle_KuralIhlali_NesneVeritabanindakiHalineDoner()
    {
        _servis.Ekle(YeniMusteri("05321112233"));
        var ikinci = YeniMusteri("05332223344", "Mehmet", "Demir");
        _servis.Ekle(ikinci);

        ikinci.Ad = "Değişti";
        ikinci.Telefon = "05321112233";
        Assert.Throws<BenzersizlikIhlaliException>(() => _servis.Guncelle(ikinci));

        // Reddedilen değişiklikler geri alındı; sonraki bir SaveChanges bunları yazamaz.
        Assert.Equal("Mehmet", ikinci.Ad);
        Assert.Equal("05332223344", ikinci.Telefon);
        Assert.False(_db.Context.ChangeTracker.HasChanges());
    }

    [Fact]
    public void Guncelle_OlmayanId_KayitBulunamadi()
    {
        var musteri = YeniMusteri("05321112233");
        musteri.Id = 999;

        Assert.Throws<KayitBulunamadiException>(() => _servis.Guncelle(musteri));
    }

    [Fact]
    public void IdIleGetir_OlmayanId_KayitBulunamadi()
    {
        var hata = Assert.Throws<KayitBulunamadiException>(() => _servis.IdIleGetir(42));

        Assert.Equal("Müşteri", hata.EntityAdi);
        Assert.Equal(42, hata.Id);
    }

    // --- Arama ---

    [Theory]
    [InlineData("ayş")]
    [InlineData("YILMAZ")]
    [InlineData("ayşe yıl")]
    [InlineData("532 111")]
    public void Ara_AdSoyadVeyaTelefon_Bulur(string metin)
    {
        _servis.Ekle(YeniMusteri("05321112233"));
        _servis.Ekle(YeniMusteri("05332223344", "Mehmet", "Demir"));

        var sonuc = Assert.Single(_servis.Ara(metin));
        Assert.Equal("Ayşe", sonuc.Ad);
    }

    [Fact]
    public void Ara_BosMetin_TumunuDoner()
    {
        _servis.Ekle(YeniMusteri("05321112233"));
        _servis.Ekle(YeniMusteri("05332223344", "Mehmet", "Demir"));

        Assert.Equal(2, _servis.Ara("  ").Count);
    }

    // --- Silme ---

    [Fact]
    public void Sil_GecmisiYok_Silinir()
    {
        var musteri = YeniMusteri("05321112233");
        _servis.Ekle(musteri);

        _servis.Sil(musteri.Id);

        Assert.Empty(_servis.TumunuGetir());
    }

    [Fact]
    public void Sil_KiralamaGecmisiVar_IliskiliKayitVar()
    {
        var musteri = _db.MusteriEkle();
        _db.KiralamaEkle(musteri, _db.PersonelEkle(), _db.SupEkle());

        var hata = Assert.Throws<IliskiliKayitVarException>(() => _servis.Sil(musteri.Id));
        Assert.Contains("pasif", hata.Message);
        Assert.Single(_servis.TumunuGetir());
    }

    [Fact]
    public void Sil_OlmayanId_KayitBulunamadi()
    {
        Assert.Throws<KayitBulunamadiException>(() => _servis.Sil(7));
    }
}
