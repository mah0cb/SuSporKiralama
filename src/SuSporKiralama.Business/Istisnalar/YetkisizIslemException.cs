namespace SuSporKiralama.Business.Istisnalar;

/// <summary>
/// Oturum açılmamış ya da oturumdaki personelin rolü bu işleme yetkili değil.
/// </summary>
public class YetkisizIslemException(string mesaj) : IsKuraliException(mesaj);
