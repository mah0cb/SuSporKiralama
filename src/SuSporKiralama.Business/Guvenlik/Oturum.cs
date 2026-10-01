using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Business.Guvenlik;

/// <summary>
/// Oturumdaki personelin giriş anındaki bilgileri (kopya; entity'nin kendisi tutulmaz).
/// Ac/Kapat internal: Business dışından (UI'dan) şifre doğrulanmadan oturum açılamaz,
/// oturum yalnızca GirisServisi üzerinden açılır.
/// </summary>
public class Oturum : IOturum
{
    private (int Id, string AdSoyad, Rol Rol)? _personel;

    public bool GirisYapildiMi => _personel is not null;

    public int PersonelId => Aktif.Id;
    public string AdSoyad => Aktif.AdSoyad;
    public Rol Rol => Aktif.Rol;

    private (int Id, string AdSoyad, Rol Rol) Aktif =>
        _personel ?? throw new YetkisizIslemException("Bu işlem için önce giriş yapmalısınız.");

    internal void Ac(Personel personel) => _personel = (personel.Id, personel.AdSoyad, personel.Rol);

    internal void Kapat() => _personel = null;
}
