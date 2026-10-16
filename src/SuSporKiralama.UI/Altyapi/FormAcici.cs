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
}
