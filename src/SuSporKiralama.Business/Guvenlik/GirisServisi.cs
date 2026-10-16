using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.DataAccess.Guvenlik;
using SuSporKiralama.DataAccess.Repository;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Business.Guvenlik;

/// <summary>
/// Giriş ve çıkış. Kaba kuvvet koruması GirisDenemeSayaci'nda: kilitli kullanıcı adında
/// doğru şifre de kabul edilmez. Sayaç dışarıdan verilir, uygulama boyunca tek örnektir.
/// </summary>
public class GirisServisi(IRepository<Personel> personelRepository, Oturum oturum, GirisDenemeSayaci sayac) : IGirisServisi
{

    // Hatalı kullanıcı adı ile hatalı şifre aynı mesajı alır; hangisinin yanlış olduğu belli olmaz.
    public const string HataliGirisMesaji = "Kullanıcı adı veya şifre hatalı.";

    // Kullanıcı bulunamadığında da bir şifre doğrulaması yapılır; böylece yanıt süresinden
    // kullanıcı adının var olup olmadığı anlaşılamaz.
    private static readonly string SahteHash = SifreHasher.Hashle(Guid.NewGuid().ToString());

    public void GirisYap(string kullaniciAdi, string sifre)
    {
        // Personel.KullaniciAdi küçük harfe çevrilerek saklanır; sayaç da aynı anahtarı kullanır.
        // Olmayan kullanıcı adları da sayılır, aksi halde kilitlenmemek kullanıcının olmadığını belli ederdi.
        var ad = (kullaniciAdi ?? "").Trim().ToLowerInvariant();
        sayac.KilitKontrol(ad);

        var personel = ad.Length == 0 ? null : personelRepository.Find(p => p.KullaniciAdi == ad).SingleOrDefault();
        var sifreDogru = SifreHasher.Dogrula(sifre ?? "", personel?.SifreHash ?? SahteHash);

        if (personel is null || !sifreDogru)
        {
            sayac.HataKaydet(ad);
            throw new GirisBasarisizException(HataliGirisMesaji);
        }

        // Pasiflik yalnızca şifre doğruysa söylenir; şifreyi bilmeyen bu bilgiyi öğrenemez.
        if (!personel.AktifMi)
            throw new GirisBasarisizException("Hesabınız pasif durumda. Lütfen yöneticinize başvurun.");

        sayac.Sifirla(ad);
        oturum.Ac(personel);
    }

    public void CikisYap() => oturum.Kapat();
}
