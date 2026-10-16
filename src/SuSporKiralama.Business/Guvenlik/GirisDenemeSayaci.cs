using SuSporKiralama.Business.Istisnalar;

namespace SuSporKiralama.Business.Guvenlik;

/// <summary>
/// Kaba kuvvet koruması: aynı kullanıcı adıyla art arda MaxHataliDeneme hatalı denemeden sonra
/// o ad KilitSuresi boyunca kilitlenir. GirisServisi'nden ayrı bir sınıftır, çünkü GirisServisi
/// DbContext'e bağlı olduğu için her kullanımda yeniden oluşturulabilir; sayaç ise uygulama
/// boyunca tek olmalıdır (DI'da singleton). Zaman TimeProvider'dan UTC olarak alınır.
/// </summary>
public class GirisDenemeSayaci(TimeProvider zaman)
{
    public const int MaxHataliDeneme = 5;
    public static readonly TimeSpan KilitSuresi = TimeSpan.FromMinutes(5);

    // NOT: sayaç bellekte; uygulama tek bilgisayarda çalıştığı için yeterli,
    // uygulama yeniden başlatılınca sıfırlanır. Kalıcı olması gerekirse Personel tablosuna taşınır.
    private readonly Dictionary<string, (int Hata, DateTimeOffset? KilitBitis)> _denemeler = [];

    /// <summary>Kullanıcı adı kilitliyse kalan süreyle GirisBasarisizException fırlatır.</summary>
    public void KilitKontrol(string ad)
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

    public void HataKaydet(string ad)
    {
        var hata = _denemeler.GetValueOrDefault(ad).Hata + 1;
        DateTimeOffset? kilitBitis = hata >= MaxHataliDeneme ? zaman.GetUtcNow() + KilitSuresi : null;
        _denemeler[ad] = (hata, kilitBitis);
    }

    public void Sifirla(string ad) => _denemeler.Remove(ad);
}
