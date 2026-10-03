namespace SuSporKiralama.Business.Oneri;

public interface IOneriServisi
{
    /// <summary>
    /// Verilen aralıkta müsait ekipmanlardan bir paket önerir. KiralamaIslemleri yetkisi gerekir.
    /// Öneri kiralamaya dönüştürülmez; bunu personel arayüzde onaylar.
    /// </summary>
    Task<OneriSonucu> OneriAlAsync(OneriIstegi istek, CancellationToken iptal = default);
}
