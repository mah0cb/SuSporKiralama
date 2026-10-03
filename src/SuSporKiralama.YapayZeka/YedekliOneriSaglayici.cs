using SuSporKiralama.Business.Oneri;

namespace SuSporKiralama.YapayZeka;

/// <summary>
/// Decorator: kendisi de bir IAiOneriSaglayici'dır ve başka bir sağlayıcıyı sarar. Önce asıl
/// sağlayıcıyı (Claude) dener; kullanılamazsa (anahtar yok, HTTP hatası, geçersiz yanıt) ya da
/// zaman aşımı dolarsa yedeğe (kural tabanlı) geçer ve sonuca bunu yazar. OneriServisi hiçbir
/// şey değişmeden bu sınıfı, Claude'u ya da kural tabanlıyı alabilir (Strategy).
/// </summary>
public class YedekliOneriSaglayici(
    IAiOneriSaglayici asil,
    IAiOneriSaglayici yedek,
    TimeSpan zamanAsimi,
    TimeProvider? zaman = null) : IAiOneriSaglayici
{
    public const string YedekUyarisi = "Yapay zeka şu an kullanılamıyor, kural tabanlı öneri gösteriliyor.";

    public async Task<HamOneri> OneriUretAsync(OneriGirdisi girdi, CancellationToken iptal = default)
    {
        // Zaman aşımı TimeProvider'dan; testlerde FakeTimeProvider ile beklemeden denenir.
        using var sure = new CancellationTokenSource(zamanAsimi, zaman ?? TimeProvider.System);
        using var birlesik = CancellationTokenSource.CreateLinkedTokenSource(iptal, sure.Token);

        try
        {
            return await asil.OneriUretAsync(girdi, birlesik.Token);
        }
        catch (AiSaglayiciException)
        {
            // Asıl sağlayıcı kullanılamıyor; yedeğe geçilir.
        }
        catch (OperationCanceledException) when (!iptal.IsCancellationRequested)
        {
            // Çağıran iptal etmedi, yani zaman aşımı doldu. Çağıranın kendi iptali yukarı aynen gider.
        }

        var oneri = await yedek.OneriUretAsync(girdi, iptal);
        return oneri with { Uyari = YedekUyarisi };
    }
}
