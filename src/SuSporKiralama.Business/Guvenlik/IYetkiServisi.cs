namespace SuSporKiralama.Business.Guvenlik;

public interface IYetkiServisi
{
    /// <summary>Oturum yoksa veya rol yetkili değilse YetkisizIslemException fırlatır.</summary>
    void YetkiKontrol(Islem islem);

    /// <summary>Exception fırlatmaz; arayüzün buton gizlemek/devre dışı bırakmak için kullanması içindir.</summary>
    bool YetkisiVarMi(Islem islem);
}
