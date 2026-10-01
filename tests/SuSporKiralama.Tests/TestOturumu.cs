using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Tests;

/// <summary>
/// Testlerde şifre doğrulamadan oturum açar (Oturum.Ac internal, InternalsVisibleTo ile erişilir).
/// Personel verilmezse veritabanına kaydedilmemiş (Id = 0) bir personel kullanılır; bu yalnızca
/// yetki kontrolü için yeterlidir. Kiralama gibi personelin kayıtlı olması gereken testlerde
/// kayıtlı personel verilir.
/// </summary>
public static class TestOturumu
{
    public static Oturum AdminOlarakGiris(Personel? personel = null) =>
        Ac(personel ?? new Personel("Test Yönetici", "test-admin", "test-hash", Rol.Admin));

    public static Oturum PersonelOlarakGiris(Personel? personel = null) =>
        Ac(personel ?? new Personel("Test Personel", "test-personel", "test-hash", Rol.Personel));

    private static Oturum Ac(Personel personel)
    {
        var oturum = new Oturum();
        oturum.Ac(personel);
        return oturum;
    }

    public static YetkiServisi Yetki(this Oturum oturum) => new(oturum);
}
