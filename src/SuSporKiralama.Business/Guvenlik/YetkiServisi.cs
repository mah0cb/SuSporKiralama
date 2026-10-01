using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Business.Guvenlik;

public class YetkiServisi(IOturum oturum) : IYetkiServisi
{
    /// <summary>
    /// Yetki matrisi: hangi rol hangi işlemleri yapabilir. Yetkiler yalnızca burada tanımlanır.
    /// Admin her işlemi yapar (yeni eklenen işlemler de otomatik dahil olur).
    /// </summary>
    private static readonly Dictionary<Rol, HashSet<Islem>> Matris = new()
    {
        [Rol.Admin] = [.. Enum.GetValues<Islem>()],
        [Rol.Personel] =
        [
            Islem.MusteriIslemleri,
            Islem.EkipmanDurumDegistirme,
            Islem.KiralamaIslemleri,
            Islem.OdemeAlma
        ]
    };

    public bool YetkisiVarMi(Islem islem) =>
        oturum.GirisYapildiMi && Matris.TryGetValue(oturum.Rol, out var islemler) && islemler.Contains(islem);

    public void YetkiKontrol(Islem islem)
    {
        if (!oturum.GirisYapildiMi)
            throw new YetkisizIslemException("Bu işlem için önce giriş yapmalısınız.");
        if (!YetkisiVarMi(islem))
            throw new YetkisizIslemException("Bu işlem için yetkiniz yok. Lütfen bir yöneticiye başvurun.");
    }
}
