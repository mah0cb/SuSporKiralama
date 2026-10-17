using Microsoft.Extensions.DependencyInjection;

namespace SuSporKiralama.UI.Altyapi;

/// <summary>
/// Formları ve sekme ekranlarını kendi DI kapsamında (scope) oluşturur. Böylece her ekranın kendi
/// DbContext'i ve servisleri olur; bir ekranda takip edilen eski veri başka bir ekrana taşınmaz.
/// Ekran kapatılıp dispose edildiğinde kapsamı (ve içindeki DbContext) de dispose edilir.
/// </summary>
public sealed class FormAcici(IServiceScopeFactory scopeFactory)
{
    public T Olustur<T>() where T : Control
    {
        var scope = scopeFactory.CreateScope();
        try
        {
            var kontrol = scope.ServiceProvider.GetRequiredService<T>();
            kontrol.Disposed += (_, _) => scope.Dispose();
            return kontrol;
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Servisi yeni ve kısa ömürlü bir kapsamda (kendi DbContext'iyle) çalıştırır, iş bitince kapsamı kapatır.
    /// Liste ekranları her işlemi böyle yapar: uzun süre açık kalan bir DbContext'te eski (takip edilen)
    /// nesneler birikmez, liste her yenilemede veritabanının güncel halini gösterir.
    /// </summary>
    public void Calistir<TServis>(Action<TServis> islem) where TServis : notnull
    {
        using var scope = scopeFactory.CreateScope();
        islem(scope.ServiceProvider.GetRequiredService<TServis>());
    }

    /// <inheritdoc cref="Calistir{TServis}(Action{TServis})"/>
    public TSonuc Getir<TServis, TSonuc>(Func<TServis, TSonuc> islem) where TServis : notnull
    {
        using var scope = scopeFactory.CreateScope();
        return islem(scope.ServiceProvider.GetRequiredService<TServis>());
    }
}
