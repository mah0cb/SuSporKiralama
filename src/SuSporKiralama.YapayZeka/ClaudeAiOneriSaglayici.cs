using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using SuSporKiralama.Business.Oneri;

namespace SuSporKiralama.YapayZeka;

/// <summary>
/// Anthropic Messages API'sini (POST /v1/messages) HttpClient ile çağırır. HttpClient dışarıdan
/// verilir; testlerde sahte bir HttpMessageHandler ile gerçek ağ çağrısı yapılmaz.
/// Her türlü kullanılamaz durum AiSaglayiciException olur; iptal (OperationCanceledException) aynen yükselir.
/// </summary>
public class ClaudeAiOneriSaglayici(HttpClient http, ClaudeAyarlari ayarlar) : IAiOneriSaglayici
{
    public const string Adres = "https://api.anthropic.com/v1/messages";
    public const string ApiSurumu = "2023-06-01";

    // Efor düşük olsa da düşünme token'ları max_tokens'a dahildir; yanıtın yarıda kesilmemesi için pay bırakılır.
    private const int MaxToken = 4096;

    // Kullanıcı mesajındaki JSON: Türkçe karakterler okunur kalsın, null alanlar yazılmasın.
    private static readonly JsonSerializerOptions MesajSecenekleri = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    // Modelin yanıtı katı ayrıştırılır: eksik alan, fazladan alan ya da yanlış tür hata sayılır.
    private static readonly JsonSerializerOptions YanitSecenekleri = new()
    {
        RespectRequiredConstructorParameters = true,
        RespectNullableAnnotations = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    public string Ad => $"Claude ({ayarlar.Model})";

    public async Task<HamOneri> OneriUretAsync(OneriGirdisi girdi, CancellationToken iptal = default)
    {
        if (!ayarlar.AnahtarVar)
            throw new AiSaglayiciException("Claude API anahtarı tanımlı değil.");

        using var istek = new HttpRequestMessage(HttpMethod.Post, Adres)
        {
            Content = new StringContent(IstekGovdesi(girdi), Encoding.UTF8, "application/json")
        };
        // Anahtar yalnızca bu isteğin header'ına konur (HttpClient.DefaultRequestHeaders'a değil).
        istek.Headers.Add("x-api-key", ayarlar.ApiAnahtari);
        istek.Headers.Add("anthropic-version", ApiSurumu);

        string yanitMetni;
        try
        {
            using var yanit = await http.SendAsync(istek, iptal);
            if (!yanit.IsSuccessStatusCode)
                throw new AiSaglayiciException($"Claude API hata döndürdü (HTTP {(int)yanit.StatusCode}).");
            yanitMetni = await yanit.Content.ReadAsStringAsync(iptal);
        }
        catch (HttpRequestException ex)
        {
            throw new AiSaglayiciException("Claude API'ye bağlanılamadı.", ex);
        }
        catch (TaskCanceledException ex) when (!iptal.IsCancellationRequested)
        {
            // Çağıran iptal etmediyse bu HttpClient.Timeout'tur.
            throw new AiSaglayiciException("Claude API zaman aşımına uğradı.", ex);
        }

        return YanitiAyristir(yanitMetni);
    }

    private string IstekGovdesi(OneriGirdisi girdi)
    {
        var format = new JsonObject { ["type"] = "json_schema", ["schema"] = ClaudePromptu.Sema() };
        var outputConfig = new JsonObject { ["format"] = format };
        if (ayarlar.Efor is not null)
            outputConfig["effort"] = ayarlar.Efor;

        var govde = new JsonObject
        {
            ["model"] = ayarlar.Model,
            ["max_tokens"] = MaxToken,
            ["system"] = ClaudePromptu.Sistem,
            ["messages"] = new JsonArray(new JsonObject { ["role"] = "user", ["content"] = KullaniciMesaji(girdi) }),
            ["output_config"] = outputConfig
        };
        return govde.ToJsonString();
    }

    // Müşteri metni ve ekipman listesi veri olarak (JSON) verilir; metin talimat gibi okunmasın.
    private static string KullaniciMesaji(OneriGirdisi girdi) => JsonSerializer.Serialize(new
    {
        musteri_metni = girdi.MusteriMetni,
        deneyim_seviyesi = girdi.Deneyim?.ToString(),
        kiralama_suresi_saat = Math.Round(girdi.Sure.TotalHours, 1),
        musait_ekipmanlar = girdi.MusaitEkipmanlar.Select(e => new
        {
            kod = e.Kod,
            tur = e.Tur,
            birim_ucret = e.BirimUcret,
            ucret_birimi = e.UcretBirimi,
            uzunluk_cm = e.UzunlukCm,
            max_tasima_kg = e.MaxTasimaKg,
            sup_tipi = e.SupTipi?.ToString(),
            kisi_kapasitesi = e.KisiKapasitesi,
            beden = e.Beden?.ToString()
        })
    }, MesajSecenekleri);

    private HamOneri YanitiAyristir(string yanitMetni)
    {
        MesajYaniti mesaj;
        ModelYaniti oneri;
        try
        {
            mesaj = JsonSerializer.Deserialize<MesajYaniti>(yanitMetni)
                    ?? throw new AiSaglayiciException("Claude yanıtı boş.");

            if (mesaj.StopReason == "refusal")
                throw new AiSaglayiciException("Claude isteği yanıtlamayı reddetti.");
            if (mesaj.StopReason == "max_tokens")
                throw new AiSaglayiciException("Claude yanıtı yarıda kesildi.");

            // Düşünme blokları da gelebilir; JSON metin bloğundadır.
            var metin = mesaj.Content?.FirstOrDefault(b => b.Type == "text")?.Text
                        ?? throw new AiSaglayiciException("Claude yanıtında metin bulunamadı.");
            oneri = JsonSerializer.Deserialize<ModelYaniti>(metin, YanitSecenekleri)
                    ?? throw new AiSaglayiciException("Claude yanıtı boş.");
        }
        catch (JsonException ex)
        {
            throw new AiSaglayiciException("Claude yanıtı beklenen JSON şemasına uymuyor.", ex);
        }

        return new HamOneri(oneri.EkipmanKodlari, oneri.KisiSayisi, oneri.Aciklama, oneri.GuvenlikNotu, Ad);
    }

    // Messages API yanıtının kullanılan kısmı (diğer alanlar yok sayılır).
    private sealed record MesajYaniti(
        [property: JsonPropertyName("content")] List<IcerikBlogu>? Content,
        [property: JsonPropertyName("stop_reason")] string? StopReason);

    private sealed record IcerikBlogu(
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("text")] string? Text);

    // Sistem promptundaki şemanın C# karşılığı.
    private sealed record ModelYaniti(
        [property: JsonPropertyName("ekipman_kodlari")] List<string> EkipmanKodlari,
        [property: JsonPropertyName("kisi_sayisi")] int KisiSayisi,
        [property: JsonPropertyName("aciklama")] string Aciklama,
        [property: JsonPropertyName("guvenlik_notu")] string GuvenlikNotu);
}
