using Microsoft.Extensions.Time.Testing;
using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.DataAccess.Guvenlik;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Tests;

public class GirisServisiTestleri : IDisposable
{
    private const string Sifre = "Guvenli123!";

    private readonly TestVeritabani _db = new();
    private readonly FakeTimeProvider _zaman = new(new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero));
    private readonly Oturum _oturum = new();
    private readonly GirisServisi _servis;
    private readonly Personel _deniz;

    public GirisServisiTestleri()
    {
        _servis = new GirisServisi(_db.Repo<Personel>(), _oturum, _zaman);
        _deniz = new Personel("Deniz Kaya", "deniz", SifreHasher.Hashle(Sifre), Rol.Personel);
        _db.Context.Personeller.Add(_deniz);
        _db.Context.SaveChanges();
    }

    public void Dispose() => _db.Dispose();

    private string HataMesaji(string kullaniciAdi, string sifre) =>
        Assert.Throws<GirisBasarisizException>(() => _servis.GirisYap(kullaniciAdi, sifre)).Message;

    private void HataliDene(int kez)
    {
        for (var i = 0; i < kez; i++)
            HataMesaji("deniz", "yanlis-sifre");
    }

    // --- Giriş ---

    [Fact]
    public void DogruGiris_OturumAcilir()
    {
        _servis.GirisYap("deniz", Sifre);

        Assert.True(_oturum.GirisYapildiMi);
        Assert.Equal(_deniz.Id, _oturum.PersonelId);
        Assert.Equal("Deniz Kaya", _oturum.AdSoyad);
        Assert.Equal(Rol.Personel, _oturum.Rol);
    }

    [Fact]
    public void KullaniciAdiBuyukHarfVeBosluklu_GirisYapilir()
    {
        _servis.GirisYap("  DENIZ ", Sifre);

        Assert.True(_oturum.GirisYapildiMi);
    }

    [Fact]
    public void YanlisSifreVeOlmayanKullanici_AyniMesaj()
    {
        var yanlisSifre = HataMesaji("deniz", "yanlis-sifre");
        var olmayanKullanici = HataMesaji("olmayan", Sifre);

        Assert.Equal(GirisServisi.HataliGirisMesaji, yanlisSifre);
        Assert.Equal(yanlisSifre, olmayanKullanici);
        Assert.False(_oturum.GirisYapildiMi);
    }

    [Fact]
    public void PasifPersonel_AyriMesajOturumAcilmaz()
    {
        _deniz.AktifMi = false;
        _db.Context.SaveChanges();

        var mesaj = HataMesaji("deniz", Sifre);

        Assert.Contains("pasif", mesaj);
        Assert.NotEqual(GirisServisi.HataliGirisMesaji, mesaj);
        Assert.False(_oturum.GirisYapildiMi);
    }

    [Fact]
    public void CikisYap_OturumKapanir()
    {
        _servis.GirisYap("deniz", Sifre);

        _servis.CikisYap();

        Assert.False(_oturum.GirisYapildiMi);
        Assert.Throws<YetkisizIslemException>(() => _oturum.PersonelId);
    }

    // --- Kaba kuvvet koruması ---

    [Fact]
    public void BesHataliDeneme_DogruSifreDeReddedilir()
    {
        HataliDene(5);

        var mesaj = HataMesaji("deniz", Sifre);

        Assert.Contains("5 dakika", mesaj);
        Assert.False(_oturum.GirisYapildiMi);
    }

    [Fact]
    public void DortHataliDeneme_HenuzKilitYok()
    {
        HataliDene(4);

        _servis.GirisYap("deniz", Sifre);

        Assert.True(_oturum.GirisYapildiMi);
    }

    [Fact]
    public void Kilit_BesDakikaSonraAcilir()
    {
        HataliDene(5);

        _zaman.Advance(TimeSpan.FromMinutes(4) + TimeSpan.FromSeconds(59));
        Assert.Contains("1 dakika", HataMesaji("deniz", Sifre));

        _zaman.Advance(TimeSpan.FromSeconds(1));
        _servis.GirisYap("deniz", Sifre);

        Assert.True(_oturum.GirisYapildiMi);
    }

    [Fact]
    public void BasariliGiris_SayaciSifirlar()
    {
        HataliDene(4);
        _servis.GirisYap("deniz", Sifre);
        HataliDene(4);

        _servis.GirisYap("deniz", Sifre); // toplam 8 hata ama art arda değil: kilit yok

        Assert.True(_oturum.GirisYapildiMi);
    }

    [Fact]
    public void Kilit_KullaniciAdinaOzel()
    {
        var ali = new Personel("Ali Veli", "ali", SifreHasher.Hashle(Sifre), Rol.Admin);
        _db.Context.Personeller.Add(ali);
        _db.Context.SaveChanges();
        HataliDene(5);

        _servis.GirisYap("ali", Sifre);

        Assert.Equal(ali.Id, _oturum.PersonelId);
    }

    [Fact]
    public void OlmayanKullaniciAdi_DeKilitlenir()
    {
        for (var i = 0; i < 5; i++)
            Assert.Equal(GirisServisi.HataliGirisMesaji, HataMesaji("olmayan", "x"));

        Assert.NotEqual(GirisServisi.HataliGirisMesaji, HataMesaji("olmayan", "x"));
    }

    // --- SifreHasher.Dogrula ---

    [Fact]
    public void SifreDogrula_DogruSifre_True()
    {
        Assert.True(SifreHasher.Dogrula(Sifre, SifreHasher.Hashle(Sifre)));
    }

    [Theory]
    [InlineData("guvenli123!")] // büyük/küçük harf duyarlı
    [InlineData("Guvenli123")]
    [InlineData("")]
    public void SifreDogrula_YanlisSifre_False(string sifre)
    {
        Assert.False(SifreHasher.Dogrula(sifre, SifreHasher.Hashle(Sifre)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("test-hash")]
    [InlineData("100000.bozuk.hash")]
    public void SifreDogrula_BozukHash_False(string? hash)
    {
        Assert.False(SifreHasher.Dogrula(Sifre, hash));
    }
}
