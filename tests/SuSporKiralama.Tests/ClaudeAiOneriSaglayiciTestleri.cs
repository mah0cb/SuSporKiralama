using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using SuSporKiralama.Business.Oneri;
using SuSporKiralama.Entities;
using SuSporKiralama.YapayZeka;

namespace SuSporKiralama.Tests;

public class ClaudeAiOneriSaglayiciTestleri
{
    // Gerçek bir anahtar değil; hiçbir yere sızmadığını aramak için ayırt edici bir metin.
    private const string Anahtar = "sk-ant-test-GIZLI-ANAHTAR-123";

    private static readonly OneriGirdisi Girdi = new(
        "2 kişiyiz, ilk kez deneyeceğiz",
        DeneyimSeviyesi.Baslangic,
        TimeSpan.FromHours(3),
        [
            new EkipmanOzeti("SUP-001", "SupBoard", 150m, "saatlik", UzunlukCm: 320, MaxTasimaKg: 120, SupTipi: SupBoardTipi.Sisme),
            new EkipmanOzeti("KANO-001", "Kano", 600m, "4 saatlik blok", KisiKapasitesi: 2),
            new EkipmanOzeti("YLK-001", "CanYelegi", 50m, "kiralama başı sabit", Beden: Beden.M)
        ]);

    private const string GecerliOneri =
        """{"ekipman_kodlari":["KANO-001","YLK-001"],"kisi_sayisi":2,"aciklama":"Kano dengelidir.","guvenlik_notu":"Kıyıya yakın kalın."}""";

    /// <summary>Messages API biçiminde yanıt; önde boş bir düşünme bloğu da bulunur.</summary>
    private static string MesajYaniti(string metin, string stopReason = "end_turn") => JsonSerializer.Serialize(new
    {
        id = "msg_test",
        type = "message",
        role = "assistant",
        model = ClaudeAyarlari.VarsayilanModel,
        content = new object[]
        {
            new { type = "thinking", thinking = "", signature = "imza" },
            new { type = "text", text = metin }
        },
        stop_reason = stopReason,
        usage = new { input_tokens = 100, output_tokens = 50 }
    });

    private static ClaudeAiOneriSaglayici Saglayici(SahteHttpHandler handler, string? anahtar = Anahtar, string? efor = ClaudeAyarlari.VarsayilanEfor) =>
        new(new HttpClient(handler), new ClaudeAyarlari(anahtar, efor: efor));

    private static async Task<AiSaglayiciException> HataBekle(SahteHttpHandler handler)
    {
        var hata = await Assert.ThrowsAsync<AiSaglayiciException>(() => Saglayici(handler).OneriUretAsync(Girdi));
        AnahtarIcermez(hata);
        return hata;
    }

    private static void AnahtarIcermez(Exception hata)
    {
        for (Exception? e = hata; e is not null; e = e.InnerException)
            Assert.DoesNotContain(Anahtar, e.ToString());
    }

    // --- Başarılı yanıt ve istek ---

    [Fact]
    public async Task GecerliYanit_DogruAyristirilir()
    {
        var oneri = await Saglayici(SahteHttpHandler.Yanit(MesajYaniti(GecerliOneri))).OneriUretAsync(Girdi);

        Assert.Equal(["KANO-001", "YLK-001"], oneri.EkipmanKodlari);
        Assert.Equal(2, oneri.KisiSayisi);
        Assert.Equal("Kano dengelidir.", oneri.Aciklama);
        Assert.Equal("Kıyıya yakın kalın.", oneri.GuvenlikNotu);
        Assert.Equal($"Claude ({ClaudeAyarlari.VarsayilanModel})", oneri.Saglayici);
        Assert.Null(oneri.Uyari);
    }

    [Fact]
    public async Task Istek_AdresHeaderlarModelSemaVeMusaitEkipmanlariIcerir()
    {
        var handler = SahteHttpHandler.Yanit(MesajYaniti(GecerliOneri));

        await Saglayici(handler).OneriUretAsync(Girdi);

        var (istek, govdeMetni) = Assert.Single(handler.Istekler);
        Assert.Equal(HttpMethod.Post, istek.Method);
        Assert.Equal(ClaudeAiOneriSaglayici.Adres, istek.RequestUri!.ToString());
        Assert.Equal(Anahtar, Assert.Single(istek.Headers.GetValues("x-api-key")));
        Assert.Equal("2023-06-01", Assert.Single(istek.Headers.GetValues("anthropic-version")));
        Assert.Equal("application/json", istek.Content!.Headers.ContentType!.MediaType);

        var govde = JsonNode.Parse(govdeMetni)!;
        Assert.Equal(ClaudeAyarlari.VarsayilanModel, (string?)govde["model"]);
        Assert.Equal(ClaudePromptu.Sistem, (string?)govde["system"]);
        Assert.Equal("json_schema", (string?)govde["output_config"]!["format"]!["type"]);
        Assert.Equal(4, govde["output_config"]!["format"]!["schema"]!["required"]!.AsArray().Count);
        Assert.Equal("low", (string?)govde["output_config"]!["effort"]);

        var mesaj = govde["messages"]![0]!;
        Assert.Equal("user", (string?)mesaj["role"]);
        var icerik = JsonNode.Parse((string)mesaj["content"]!)!;
        Assert.Equal(Girdi.MusteriMetni, (string?)icerik["musteri_metni"]);
        Assert.Equal("Baslangic", (string?)icerik["deneyim_seviyesi"]);
        Assert.Equal(3, (double)icerik["kiralama_suresi_saat"]!);
        var ekipmanlar = icerik["musait_ekipmanlar"]!.AsArray();
        Assert.Equal(["SUP-001", "KANO-001", "YLK-001"], ekipmanlar.Select(e => (string)e!["kod"]!));
        Assert.Equal("Sisme", (string?)ekipmanlar[0]!["sup_tipi"]);
        Assert.Equal(2, (int)ekipmanlar[1]!["kisi_kapasitesi"]!);
        Assert.Equal("M", (string?)ekipmanlar[2]!["beden"]);
        Assert.Equal(150m, (decimal)ekipmanlar[0]!["birim_ucret"]!);
        Assert.Null(ekipmanlar[2]!["kisi_kapasitesi"]); // türe ait olmayan alanlar gönderilmez
    }

    [Fact]
    public async Task Anahtar_YalnizcaHeaderdaBulunur()
    {
        var handler = SahteHttpHandler.Yanit(MesajYaniti(GecerliOneri));
        var http = new HttpClient(handler);

        await new ClaudeAiOneriSaglayici(http, new ClaudeAyarlari(Anahtar)).OneriUretAsync(Girdi);

        var (istek, govde) = Assert.Single(handler.Istekler);
        Assert.DoesNotContain(Anahtar, govde);
        Assert.DoesNotContain(Anahtar, istek.RequestUri!.ToString());
        Assert.False(http.DefaultRequestHeaders.Contains("x-api-key"));
        Assert.Equal(Anahtar, Assert.Single(istek.Headers.GetValues("x-api-key")));
    }

    [Fact]
    public async Task EforBos_EffortAlaniGonderilmez()
    {
        var handler = SahteHttpHandler.Yanit(MesajYaniti(GecerliOneri));

        await Saglayici(handler, efor: null).OneriUretAsync(Girdi);

        var outputConfig = JsonNode.Parse(handler.Istekler[0].Govde)!["output_config"]!.AsObject();
        Assert.False(outputConfig.ContainsKey("effort"));
        Assert.True(outputConfig.ContainsKey("format"));
    }

    // --- Hatalar ---

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AnahtarYok_IstekGonderilmeden_AiSaglayiciException(string? anahtar)
    {
        var handler = SahteHttpHandler.Yanit(MesajYaniti(GecerliOneri));

        await Assert.ThrowsAsync<AiSaglayiciException>(() => Saglayici(handler, anahtar).OneriUretAsync(Girdi));
        Assert.Empty(handler.Istekler);
    }

    [Theory]
    [InlineData("bu bir JSON değil")]
    [InlineData("")]
    [InlineData("""{"ekipman_kodlari":["KANO-001"],"kisi_sayisi":2,"aciklama":"x"}""")]                                  // eksik alan
    [InlineData("""{"ekipman_kodlari":["KANO-001"],"kisi_sayisi":2,"aciklama":"x","guvenlik_notu":"y","fiyat":100}""")]  // fazladan alan
    [InlineData("""{"ekipman_kodlari":["KANO-001"],"kisi_sayisi":"iki","aciklama":"x","guvenlik_notu":"y"}""")]          // yanlış tür
    [InlineData("""{"ekipman_kodlari":null,"kisi_sayisi":2,"aciklama":"x","guvenlik_notu":"y"}""")]                       // null
    [InlineData("""["KANO-001"]""")]                                                                                       // nesne değil
    public async Task SemayaUymayanModelYaniti_AiSaglayiciException(string metin)
    {
        await HataBekle(SahteHttpHandler.Yanit(MesajYaniti(metin)));
    }

    [Theory]
    [InlineData("<html>Bakımda</html>")]
    [InlineData("""{"content":[{"type":"thinking","thinking":""}],"stop_reason":"end_turn"}""")] // metin bloğu yok
    public async Task MesajYanitiOkunamaz_AiSaglayiciException(string govde)
    {
        await HataBekle(SahteHttpHandler.Yanit(govde));
    }

    [Theory]
    [InlineData("refusal")]
    [InlineData("max_tokens")]
    public async Task KullanilamazStopReason_AiSaglayiciException(string stopReason)
    {
        await HataBekle(SahteHttpHandler.Yanit(MesajYaniti(GecerliOneri, stopReason)));
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData((HttpStatusCode)529)] // overloaded
    public async Task HttpHatasi_AiSaglayiciException_DurumKoduMesajda(HttpStatusCode durum)
    {
        // Sunucu hata gövdesinde anahtarı geri yansıtsa bile mesaja gövde yazılmaz.
        var govde = $$$"""{"type":"error","error":{"type":"x","message":"anahtar {{{Anahtar}}} geçersiz"}}""";

        var hata = await HataBekle(SahteHttpHandler.Yanit(govde, durum));

        Assert.Contains($"HTTP {(int)durum}", hata.Message);
    }

    [Fact]
    public async Task BaglantiHatasi_AiSaglayiciException()
    {
        await HataBekle(new SahteHttpHandler((_, _) => throw new HttpRequestException("Ağa ulaşılamıyor")));
    }

    [Fact]
    public async Task HttpClientZamanAsimi_AiSaglayiciException()
    {
        var http = new HttpClient(SahteHttpHandler.Cevapsiz()) { Timeout = TimeSpan.FromMilliseconds(50) };

        var hata = await Assert.ThrowsAsync<AiSaglayiciException>(() =>
            new ClaudeAiOneriSaglayici(http, new ClaudeAyarlari(Anahtar)).OneriUretAsync(Girdi));

        Assert.Contains("zaman aşımı", hata.Message);
        AnahtarIcermez(hata);
    }

    [Fact]
    public async Task CagiranIptalEderse_OperationCanceled_Sarmalanmaz()
    {
        using var iptal = new CancellationTokenSource();
        var gorev = Saglayici(SahteHttpHandler.Cevapsiz()).OneriUretAsync(Girdi, iptal.Token);
        iptal.Cancel();

        var hata = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => gorev);
        AnahtarIcermez(hata);
    }

    // --- Ayarlar ---

    [Fact]
    public void Ayarlar_ToStringAnahtariYazmaz()
    {
        var ayarlar = new ClaudeAyarlari(Anahtar);

        Assert.DoesNotContain(Anahtar, ayarlar.ToString());
        Assert.Contains("tanımlı", ayarlar.ToString());
    }

    [Fact]
    public void Ayarlar_Varsayilanlar()
    {
        var ayarlar = new ClaudeAyarlari(null);

        Assert.False(ayarlar.AnahtarVar);
        Assert.Equal(ClaudeAyarlari.VarsayilanModel, ayarlar.Model);
        Assert.Equal("low", ayarlar.Efor);
        Assert.Equal(TimeSpan.FromSeconds(20), ayarlar.ZamanAsimi);
    }

    [Fact]
    public void Yukle_OrtamDegiskeniDosyadakiAnahtariEzer_DigerAlanlarDosyadan()
    {
        using var klasor = new GeciciAyarKlasoru("""
            { "Claude": { "ApiAnahtari": "dosyadaki-anahtar", "Model": "claude-haiku-4-5", "Efor": "", "ZamanAsimiSaniye": 7 } }
            """);

        var ortamdan = ClaudeAyarlari.Yukle(klasor.Yol, "ortamdaki-anahtar");
        var dosyadan = ClaudeAyarlari.Yukle(klasor.Yol, null);

        Assert.Equal("ortamdaki-anahtar", ortamdan.ApiAnahtari);
        Assert.Equal("dosyadaki-anahtar", dosyadan.ApiAnahtari);
        Assert.Equal("claude-haiku-4-5", dosyadan.Model);
        Assert.Null(dosyadan.Efor);                            // boş bırakılan efor gönderilmez
        Assert.Equal(TimeSpan.FromSeconds(7), dosyadan.ZamanAsimi);
    }

    [Fact]
    public void Yukle_DosyaYoksa_Varsayilanlar()
    {
        using var klasor = new GeciciAyarKlasoru(null);

        var ayarlar = ClaudeAyarlari.Yukle(klasor.Yol, null);

        Assert.False(ayarlar.AnahtarVar);
        Assert.Equal(ClaudeAyarlari.VarsayilanModel, ayarlar.Model);
        Assert.Equal(ClaudeAyarlari.VarsayilanEfor, ayarlar.Efor);
    }

    private sealed class GeciciAyarKlasoru : IDisposable
    {
        public string Yol { get; } = Directory.CreateTempSubdirectory("susporkiralama-").FullName;

        public GeciciAyarKlasoru(string? appsettings)
        {
            if (appsettings is not null)
                File.WriteAllText(Path.Combine(Yol, "appsettings.json"), appsettings);
        }

        public void Dispose() => Directory.Delete(Yol, recursive: true);
    }
}
