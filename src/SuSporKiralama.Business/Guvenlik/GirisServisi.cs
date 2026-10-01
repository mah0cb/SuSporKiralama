using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.DataAccess.Guvenlik;
using SuSporKiralama.DataAccess.Repository;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Business.Guvenlik;

/// <summary>
/// Giriş ve çıkış. Kaba kuvvet koruması: aynı kullanıcı adıyla art arda MaxHataliDeneme hatalı
/// denemeden sonra o ad KilitSuresi boyunca kilitlenir (doğru şifre de kabul edilmez).
/// Zaman TimeProvider'dan UTC olarak alınır; yaz saati değişimi kilidi etkilemez.
/// </summary>
public class GirisServisi(IRepository<Personel> personelRepository, Oturum oturum, TimeProvider zaman) : IGirisServisi
{
    public const int MaxHataliDeneme = 5;
    public static readonly TimeSpan KilitSuresi = TimeSpan.FromMinutes(5);

    // Hatalı kullanıcı adı ile hatalı şifre aynı mesajı alır; hangisinin yanlış olduğu belli olmaz.
    public const string HataliGirisMesaji = "Kullanıcı adı veya şifre hatalı.";

    // Kullanıcı bulunamadığında da bir şifre doğrulaması yapılır; böylece yanıt süresinden
    // kullanıcı adının var olup olmadığı anlaşılamaz.
    private static readonly string SahteHash = SifreHasher.Hashle(Guid.NewGuid().ToString());

    // ponytail: sayaç bellekte; uygulama tek bilgisayarda çalıştığı için yeterli,
    // uygulama yeniden başlatılınca sıfırlanır. Kalıcı olması gerekirse Personel tablosuna taşınır.
    private readonly Dictionary<string, (int Hata, DateTimeOffset? KilitBitis)> _denemeler = [];

    public void GirisYap(string kullaniciAdi, string sifre)
    {
        // Personel.KullaniciAdi küçük harfe çevrilerek saklanır; sayaç da aynı anahtarı kullanır.
        // Olmayan kullanıcı adları da sayılır, aksi halde kilitlenmemek kullanıcının olmadığını belli ederdi.
        var ad = (kullaniciAdi ?? "").Trim().ToLowerInvariant();
        KilitKontrol(ad);

        var personel = ad.Length == 0 ? null : personelRepository.Find(p => p.KullaniciAdi == ad).SingleOrDefault();
        var sifreDogru = SifreHasher.Dogrula(sifre ?? "", personel?.SifreHash ?? SahteHash);

        if (personel is null || !sifreDogru)
        {
            HataliDenemeKaydet(ad);
            throw new GirisBasarisizException(HataliGirisMesaji);
        }

        // Pasiflik yalnızca şifre doğruysa söylenir; şifreyi bilmeyen bu bilgiyi öğrenemez.
        if (!personel.AktifMi)
            throw new GirisBasarisizException("Hesabınız pasif durumda. Lütfen yöneticinize başvurun.");

        _denemeler.Remove(ad);
        oturum.Ac(personel);
    }

    public void CikisYap() => oturum.Kapat();

    private void KilitKontrol(string ad)
    {
        if (!_denemeler.TryGetValue(ad, out var kayit) || kayit.KilitBitis is null)
            return;

        var kalan = kayit.KilitBitis.Value - zaman.GetUtcNow();
        if (kalan <= TimeSpan.Zero)
        {
            _denemeler.Remove(ad); // süre doldu: yeni 5 deneme hakkı
            return;
        }

        var dakika = (int)Math.Ceiling(kalan.TotalMinutes);
        throw new GirisBasarisizException($"Çok fazla hatalı deneme yapıldı. {dakika} dakika sonra tekrar deneyin.");
    }

    private void HataliDenemeKaydet(string ad)
    {
        var hata = _denemeler.GetValueOrDefault(ad).Hata + 1;
        DateTimeOffset? kilitBitis = hata >= MaxHataliDeneme ? zaman.GetUtcNow() + KilitSuresi : null;
        _denemeler[ad] = (hata, kilitBitis);
    }
}
