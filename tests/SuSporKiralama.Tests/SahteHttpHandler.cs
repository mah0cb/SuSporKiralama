using System.Net;
using System.Text;

namespace SuSporKiralama.Tests;

/// <summary>
/// Gerçek ağa çıkmayan HttpMessageHandler: gelen istekleri (gövdesiyle) kaydeder, testin verdiği yanıtı döndürür.
/// </summary>
public class SahteHttpHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> yanitla) : HttpMessageHandler
{
    public List<(HttpRequestMessage Istek, string Govde)> Istekler { get; } = [];

    public static SahteHttpHandler Yanit(string govde, HttpStatusCode durum = HttpStatusCode.OK) =>
        new((_, _) => Task.FromResult(new HttpResponseMessage(durum)
        {
            Content = new StringContent(govde, Encoding.UTF8, "application/json")
        }));

    /// <summary>İptal edilene kadar yanıt vermeyen sunucu (zaman aşımı senaryoları için).</summary>
    public static SahteHttpHandler Cevapsiz() =>
        new(async (_, iptal) =>
        {
            await Task.Delay(Timeout.Infinite, iptal);
            throw new InvalidOperationException("Buraya ulaşılmamalı.");
        });

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var govde = request.Content is null ? "" : await request.Content.ReadAsStringAsync(cancellationToken);
        Istekler.Add((request, govde));
        return await yanitla(request, cancellationToken);
    }
}
