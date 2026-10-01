using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Tests;

public class PersonelServisiTestleri : IDisposable
{
    private const string GecerliSifre = "Guvenli123!";

    private readonly TestVeritabani _db = new();
    private readonly PersonelServisi _servis;

    public PersonelServisiTestleri()
    {
        // Oturumdaki yönetici veritabanına kaydedilmez; "son aktif admin" testlerini etkilemez.
        _servis = new PersonelServisi(_db.Repo<Personel>(), _db.Repo<Kiralama>(), TestOturumu.AdminOlarakGiris().Yetki());
    }

    public void Dispose() => _db.Dispose();

    private Personel AdminEkle(string kullaniciAdi = "admin") =>
        _servis.Ekle("Sistem Yöneticisi", kullaniciAdi, GecerliSifre, Rol.Admin);

    private Personel PersonelEkle(string kullaniciAdi = "deniz") =>
        _servis.Ekle("Deniz Kaya", kullaniciAdi, GecerliSifre, Rol.Personel);

    // --- Ekleme ve şifre ---

    [Fact]
    public void Ekle_SifreHashlenerekSaklanir()
    {
        var personel = PersonelEkle();

        var kayitli = _servis.IdIleGetir(personel.Id);
        Assert.NotEqual(GecerliSifre, kayitli.SifreHash);
        Assert.DoesNotContain(GecerliSifre, kayitli.SifreHash);
        Assert.StartsWith("100000.", kayitli.SifreHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234567")]
    public void Ekle_KisaSifre_DogrulamaException(string sifre)
    {
        Assert.Throws<DogrulamaException>(() => _servis.Ekle("Deniz Kaya", "deniz", sifre, Rol.Personel));
        Assert.Empty(_servis.TumunuGetir());
    }

    [Fact]
    public void Ekle_BosAdSoyad_EntityHatasiDogrulamaExceptionIleSarilir()
    {
        var hata = Assert.Throws<DogrulamaException>(() => _servis.Ekle("  ", "deniz", GecerliSifre, Rol.Personel));

        Assert.IsType<ArgumentException>(hata.InnerException);
        Assert.Equal("AdSoyad boş olamaz.", hata.Message);
    }

    [Fact]
    public void Ekle_AyniKullaniciAdiBuyukHarf_BenzersizlikIhlali()
    {
        PersonelEkle("deniz");

        var hata = Assert.Throws<BenzersizlikIhlaliException>(() => PersonelEkle("DENIZ"));
        Assert.Equal("Kullanıcı adı", hata.AlanAdi);
    }

    [Fact]
    public void Ekle_GenelYoldaHashlenmemisSifre_DogrulamaException()
    {
        var personel = new Personel("Deniz Kaya", "deniz", "duz-sifre-123", Rol.Personel);

        var hata = Assert.Throws<DogrulamaException>(() => _servis.Ekle(personel));
        Assert.Contains("hash'lenmemiş", hata.Message);
        Assert.Empty(_servis.TumunuGetir());
    }

    [Fact]
    public void SifreDegistir_YeniHashKaydedilir()
    {
        var personel = PersonelEkle();
        var eskiHash = personel.SifreHash;

        _servis.SifreDegistir(personel.Id, "YeniSifre456!");

        Assert.NotEqual(eskiHash, _servis.IdIleGetir(personel.Id).SifreHash);
    }

    [Fact]
    public void SifreDegistir_KisaSifre_DogrulamaException()
    {
        var personel = PersonelEkle();
        var eskiHash = personel.SifreHash;

        Assert.Throws<DogrulamaException>(() => _servis.SifreDegistir(personel.Id, "kisa"));
        Assert.Equal(eskiHash, personel.SifreHash);
    }

    [Fact]
    public void SifreDegistir_NesnedekiKontrolsuzDegisiklikYazilmaz()
    {
        var admin = AdminEkle();
        admin.Rol = Rol.Personel; // Guncelle çağrılmadı, kural kontrol edilmedi

        _servis.SifreDegistir(admin.Id, "YeniSifre456!");

        Assert.Equal(Rol.Admin, _servis.IdIleGetir(admin.Id).Rol);
    }

    // --- Güncelleme ---

    [Fact]
    public void Guncelle_AdSoyad_HashAyniKalir()
    {
        var personel = PersonelEkle();
        var hash = personel.SifreHash;

        personel.AdSoyad = "Deniz Kaya Yılmaz";
        _servis.Guncelle(personel);

        var kayitli = _servis.IdIleGetir(personel.Id);
        Assert.Equal("Deniz Kaya Yılmaz", kayitli.AdSoyad);
        Assert.Equal(hash, kayitli.SifreHash);
    }

    [Fact]
    public void Guncelle_SifreHashDegistirilmis_DogrulamaException()
    {
        var personel = PersonelEkle();
        var hash = personel.SifreHash;

        personel.SifreHash = "duz-sifre";

        Assert.Throws<DogrulamaException>(() => _servis.Guncelle(personel));
        Assert.Equal(hash, personel.SifreHash);
    }

    // --- Aktiflik ve son admin kuralı ---

    [Fact]
    public void AktiflikDegistir_PersoneliPasifYap_Basarili()
    {
        AdminEkle();
        var personel = PersonelEkle();

        _servis.AktiflikDegistir(personel.Id, false);

        Assert.False(_servis.IdIleGetir(personel.Id).AktifMi);
    }

    [Fact]
    public void AktiflikDegistir_SonAktifAdmin_IslemYapilamaz()
    {
        var admin = AdminEkle();
        PersonelEkle();

        Assert.Throws<IslemYapilamazException>(() => _servis.AktiflikDegistir(admin.Id, false));
        Assert.True(admin.AktifMi);
    }

    [Fact]
    public void AktiflikDegistir_BaskaAktifAdminVarken_Basarili()
    {
        var admin = AdminEkle();
        AdminEkle("admin2");

        _servis.AktiflikDegistir(admin.Id, false);

        Assert.False(_servis.IdIleGetir(admin.Id).AktifMi);
    }

    [Fact]
    public void AktiflikDegistir_DigerAdminPasifken_SonAktifAdminKorunur()
    {
        var admin = AdminEkle();
        var admin2 = AdminEkle("admin2");
        _servis.AktiflikDegistir(admin2.Id, false);

        Assert.Throws<IslemYapilamazException>(() => _servis.AktiflikDegistir(admin.Id, false));
    }

    [Fact]
    public void Guncelle_SonAdminRolunuDusur_IslemYapilamazVeRolGeriAlinir()
    {
        var admin = AdminEkle();

        admin.Rol = Rol.Personel;

        Assert.Throws<IslemYapilamazException>(() => _servis.Guncelle(admin));
        Assert.Equal(Rol.Admin, admin.Rol);
        Assert.False(_db.Context.ChangeTracker.HasChanges());
    }

    // --- Silme ---

    [Fact]
    public void Sil_SonAktifAdmin_IslemYapilamaz()
    {
        var admin = AdminEkle();

        Assert.Throws<IslemYapilamazException>(() => _servis.Sil(admin.Id));
    }

    [Fact]
    public void Sil_GecmisiYok_Silinir()
    {
        AdminEkle();
        var personel = PersonelEkle();

        _servis.Sil(personel.Id);

        Assert.Single(_servis.TumunuGetir());
    }

    [Fact]
    public void Sil_KiralamaGecmisiVar_IliskiliKayitVar_PasifOnerilir()
    {
        AdminEkle();
        var personel = PersonelEkle();
        _db.KiralamaEkle(_db.MusteriEkle(), personel, _db.SupEkle());

        var hata = Assert.Throws<IliskiliKayitVarException>(() => _servis.Sil(personel.Id));
        Assert.Contains("pasif", hata.Message);
    }

    // --- Yetki ---

    [Fact]
    public void PersonelRolu_PersonelYonetimiYapamaz()
    {
        var deniz = PersonelEkle();
        var servis = new PersonelServisi(_db.Repo<Personel>(), _db.Repo<Kiralama>(), TestOturumu.PersonelOlarakGiris().Yetki());

        Assert.Throws<YetkisizIslemException>(() => servis.Ekle("Ali Veli", "ali", GecerliSifre, Rol.Admin));
        Assert.Throws<YetkisizIslemException>(() => servis.SifreDegistir(deniz.Id, "YeniSifre123"));
        Assert.Throws<YetkisizIslemException>(() => servis.AktiflikDegistir(deniz.Id, false));
        Assert.Throws<YetkisizIslemException>(() => servis.Sil(deniz.Id));

        Assert.Single(_servis.TumunuGetir());
        Assert.True(_servis.IdIleGetir(deniz.Id).AktifMi);
    }
}
