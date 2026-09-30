using SuSporKiralama.Business.Servisler.Soyut;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Business.Servisler;

public interface IPersonelServisi : ICrudServisi<Personel>
{
    /// <summary>
    /// Yeni personel ekler; düz şifre burada hash'lenir ve hiçbir yerde saklanmaz.
    /// Personel eklemenin önerilen yolu budur (genel Ekle(Personel) hash'lenmiş nesne bekler).
    /// </summary>
    Personel Ekle(string adSoyad, string kullaniciAdi, string sifre, Rol rol);

    /// <summary>Şifreyi değiştirir. Nesnedeki kaydedilmemiş diğer değişiklikler atılır.</summary>
    void SifreDegistir(int id, string yeniSifre);

    void AktiflikDegistir(int id, bool aktif);
}
