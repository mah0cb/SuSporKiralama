using SuSporKiralama.Entities;

namespace SuSporKiralama.Business.Guvenlik;

/// <summary>
/// Uygulamada o an giriş yapmış personel. Uygulama boyunca tek bir oturum nesnesi kullanılır;
/// servisler yalnızca okur, oturumu yalnızca GirisServisi açıp kapatır.
/// </summary>
public interface IOturum
{
    bool GirisYapildiMi { get; }

    // Oturum yokken bu üç özellik YetkisizIslemException fırlatır (0 / boş değer sessizce kullanılmasın).
    int PersonelId { get; }
    string AdSoyad { get; }
    Rol Rol { get; }
}
