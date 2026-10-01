using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Tests;

public class YetkiServisiTestleri
{
    // Beklenen matris burada ayrıca yazılı: YetkiServisi'ndeki matris yanlışlıkla değişirse test yakalar.
    [Theory]
    [InlineData(Rol.Admin, Islem.PersonelYonetimi, true)]
    [InlineData(Rol.Admin, Islem.MusteriIslemleri, true)]
    [InlineData(Rol.Admin, Islem.EkipmanYonetimi, true)]
    [InlineData(Rol.Admin, Islem.FiyatGuncelleme, true)]
    [InlineData(Rol.Admin, Islem.EkipmanDurumDegistirme, true)]
    [InlineData(Rol.Admin, Islem.KiralamaIslemleri, true)]
    [InlineData(Rol.Admin, Islem.OdemeAlma, true)]
    [InlineData(Rol.Admin, Islem.RaporGoruntuleme, true)]
    [InlineData(Rol.Personel, Islem.PersonelYonetimi, false)]
    [InlineData(Rol.Personel, Islem.MusteriIslemleri, true)]
    [InlineData(Rol.Personel, Islem.EkipmanYonetimi, false)]
    [InlineData(Rol.Personel, Islem.FiyatGuncelleme, false)]
    [InlineData(Rol.Personel, Islem.EkipmanDurumDegistirme, true)]
    [InlineData(Rol.Personel, Islem.KiralamaIslemleri, true)]
    [InlineData(Rol.Personel, Islem.OdemeAlma, true)]
    [InlineData(Rol.Personel, Islem.RaporGoruntuleme, false)]
    public void YetkiMatrisi(Rol rol, Islem islem, bool yetkili)
    {
        var oturum = rol == Rol.Admin ? TestOturumu.AdminOlarakGiris() : TestOturumu.PersonelOlarakGiris();
        var yetki = oturum.Yetki();

        Assert.Equal(yetkili, yetki.YetkisiVarMi(islem));
        if (yetkili)
            yetki.YetkiKontrol(islem);
        else
            Assert.Throws<YetkisizIslemException>(() => yetki.YetkiKontrol(islem));
    }

    [Fact]
    public void MatristeTumIslemlerTestEdiliyor()
    {
        // Yeni bir Islem eklenirse yukarıdaki Theory'ye satır eklenmesi hatırlansın.
        Assert.Equal(8, Enum.GetValues<Islem>().Length);
    }

    [Fact]
    public void OturumYok_HicbirIslemYapilamaz()
    {
        var yetki = new YetkiServisi(new Oturum());

        Assert.All(Enum.GetValues<Islem>(), islem =>
        {
            Assert.False(yetki.YetkisiVarMi(islem));
            var hata = Assert.Throws<YetkisizIslemException>(() => yetki.YetkiKontrol(islem));
            Assert.Contains("giriş", hata.Message);
        });
    }

    [Fact]
    public void OturumYok_OturumBilgisiOkunamaz()
    {
        var oturum = new Oturum();

        Assert.False(oturum.GirisYapildiMi);
        Assert.Throws<YetkisizIslemException>(() => oturum.PersonelId);
        Assert.Throws<YetkisizIslemException>(() => oturum.Rol);
    }

    [Fact]
    public void YetkisizIslemException_IsKuraliExceptionTuru()
    {
        // UI tek bir IsKuraliException catch'i ile yetki hatalarını da yakalayabilmeli.
        var yetki = TestOturumu.PersonelOlarakGiris().Yetki();

        Assert.ThrowsAny<IsKuraliException>(() => yetki.YetkiKontrol(Islem.PersonelYonetimi));
    }
}
