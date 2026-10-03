using System.Net;
using Microsoft.Extensions.Time.Testing;
using SuSporKiralama.Business.Oneri;
using SuSporKiralama.Entities;
using SuSporKiralama.YapayZeka;

namespace SuSporKiralama.Tests;

public class YedekliOneriSaglayiciTestleri
{
    private static readonly TimeSpan ZamanAsimi = TimeSpan.FromSeconds(20);

    private static readonly OneriGirdisi Girdi = new("2 kişiyiz, sakin", null, TimeSpan.FromHours(2),
    [
        new EkipmanOzeti("KANO-001", "Kano", 600m, "4 saatlik blok", KisiKapasitesi: 2),
        new EkipmanOzeti("YLK-001", "CanYelegi", 50m, "kiralama başı sabit", Beden: Beden.M),
        new EkipmanOzeti("YLK-002", "CanYelegi", 50m, "kiralama başı sabit", Beden: Beden.M)
    ]);

    private const string ClaudeYaniti = """
        {"content":[{"type":"text","text":"{\"ekipman_kodlari\":[\"KANO-001\"],\"kisi_sayisi\":2,\"aciklama\":\"a\",\"guvenlik_notu\":\"g\"}"}],"stop_reason":"end_turn"}
        """;

    private readonly FakeTimeProvider _zaman = new();

    private YedekliOneriSaglayici Yedekli(SahteHttpHandler handler, string? anahtar = "test-anahtar") =>
        new(new ClaudeAiOneriSaglayici(new HttpClient(handler), new ClaudeAyarlari(anahtar)),
            new KuralTabanliOneriSaglayici(), ZamanAsimi, _zaman);

    private static void KuralTabanliyaGecildi(HamOneri oneri)
    {
        Assert.Equal(KuralTabanliOneriSaglayici.Ad, oneri.Saglayici);
        Assert.Equal("Yapay zeka şu an kullanılamıyor, kural tabanlı öneri gösteriliyor.", oneri.Uyari);
        Assert.Equal(["KANO-001", "YLK-001", "YLK-002"], oneri.EkipmanKodlari);
    }

    [Fact]
    public async Task ClaudeBasarili_ClaudeSonucuUyarisiz()
    {
        var oneri = await Yedekli(SahteHttpHandler.Yanit(ClaudeYaniti)).OneriUretAsync(Girdi);

        Assert.StartsWith("Claude", oneri.Saglayici);
        Assert.Null(oneri.Uyari);
        Assert.Equal(["KANO-001"], oneri.EkipmanKodlari);
    }

    [Fact]
    public async Task AnahtarYok_KuralTabanliyaGecer()
    {
        var handler = SahteHttpHandler.Yanit(ClaudeYaniti);

        KuralTabanliyaGecildi(await Yedekli(handler, anahtar: null).OneriUretAsync(Girdi));
        Assert.Empty(handler.Istekler);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task HttpHatasi_KuralTabanliyaGecer(HttpStatusCode durum)
    {
        KuralTabanliyaGecildi(await Yedekli(SahteHttpHandler.Yanit("{}", durum)).OneriUretAsync(Girdi));
    }

    [Fact]
    public async Task GecersizJson_KuralTabanliyaGecer()
    {
        var yanit = """{"content":[{"type":"text","text":"Elbette! İşte önerim: kano"}],"stop_reason":"end_turn"}""";

        KuralTabanliyaGecildi(await Yedekli(SahteHttpHandler.Yanit(yanit)).OneriUretAsync(Girdi));
    }

    [Fact]
    public async Task BaglantiHatasi_KuralTabanliyaGecer()
    {
        var handler = new SahteHttpHandler((_, _) => throw new HttpRequestException("internet yok"));

        KuralTabanliyaGecildi(await Yedekli(handler).OneriUretAsync(Girdi));
    }

    [Fact]
    public async Task ZamanAsimi_KuralTabanliyaGecer()
    {
        var gorev = Yedekli(SahteHttpHandler.Cevapsiz()).OneriUretAsync(Girdi);

        _zaman.Advance(ZamanAsimi - TimeSpan.FromSeconds(1));
        await Task.Delay(50);
        Assert.False(gorev.IsCompleted);            // süre dolmadan yedeğe geçilmez

        _zaman.Advance(TimeSpan.FromSeconds(1));
        KuralTabanliyaGecildi(await gorev);
    }

    [Fact]
    public async Task CagiranIptalEderse_YedegeGecmez()
    {
        using var iptal = new CancellationTokenSource();
        var gorev = Yedekli(SahteHttpHandler.Cevapsiz()).OneriUretAsync(Girdi, iptal.Token);
        iptal.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => gorev);
    }

    [Fact]
    public async Task BeklenmeyenHata_YutulmazYukariCikar()
    {
        // Programlama hatası gibi beklenmeyen durumlar yedekle gizlenmez.
        var bozuk = new HataFirlatanSaglayici(new InvalidOperationException("hata"));
        var yedekli = new YedekliOneriSaglayici(bozuk, new KuralTabanliOneriSaglayici(), ZamanAsimi, _zaman);

        await Assert.ThrowsAsync<InvalidOperationException>(() => yedekli.OneriUretAsync(Girdi));
    }

    private sealed class HataFirlatanSaglayici(Exception hata) : IAiOneriSaglayici
    {
        public Task<HamOneri> OneriUretAsync(OneriGirdisi girdi, CancellationToken iptal = default) => Task.FromException<HamOneri>(hata);
    }
}
