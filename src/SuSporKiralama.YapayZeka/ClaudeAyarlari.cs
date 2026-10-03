using Microsoft.Extensions.Configuration;

namespace SuSporKiralama.YapayZeka;

/// <summary>
/// Claude API ayarları. Anahtar önce ANTHROPIC_API_KEY ortam değişkeninden, yoksa gitignore'daki
/// appsettings.json'ın "Claude:ApiAnahtari" alanından okunur; koda ve repoya yazılmaz.
/// record değil class: record'un otomatik ToString'i anahtarı loga yazdırırdı.
/// </summary>
public sealed class ClaudeAyarlari
{
    public const string OrtamDegiskeni = "ANTHROPIC_API_KEY";

    // NOT: Varsayılan model Sonnet 5.5: "Fast" gecikme sınıfında ve $2/$10 (milyon token) ile hızlı ve
    // görece ucuz. Daha ucuz Haiku 4.5 seçilmedi; Ekim 2026 ortasında emekliye ayrılabilir
    // ("not sooner than 15.10.2026"). Kısa bir seçim işi olduğu için düşünme eforu "low".
    public const string VarsayilanModel = "claude-sonnet-5-5";
    public const string VarsayilanEfor = "low";
    public static readonly TimeSpan VarsayilanZamanAsimi = TimeSpan.FromSeconds(20);

    /// <param name="efor">output_config.effort; null ise gönderilmez (effort desteklemeyen Haiku 4.5 gibi modeller için).</param>
    public ClaudeAyarlari(string? apiAnahtari, string? model = null, string? efor = VarsayilanEfor, TimeSpan? zamanAsimi = null)
    {
        ApiAnahtari = string.IsNullOrWhiteSpace(apiAnahtari) ? null : apiAnahtari.Trim();
        Model = string.IsNullOrWhiteSpace(model) ? VarsayilanModel : model.Trim();
        Efor = string.IsNullOrWhiteSpace(efor) ? null : efor.Trim();
        ZamanAsimi = zamanAsimi is { } sure && sure > TimeSpan.Zero ? sure : VarsayilanZamanAsimi;
    }

    public string? ApiAnahtari { get; }
    public bool AnahtarVar => ApiAnahtari is not null;
    public string Model { get; }
    public string? Efor { get; }

    /// <summary>YedekliOneriSaglayici bu süre dolunca kural tabanlı öneriye geçer.</summary>
    public TimeSpan ZamanAsimi { get; }

    // Anahtar asla yazılmaz; yalnızca tanımlı olup olmadığı.
    public override string ToString() =>
        $"Model: {Model}, Efor: {Efor ?? "-"}, Zaman aşımı: {ZamanAsimi.TotalSeconds} sn, API anahtarı: {(AnahtarVar ? "tanımlı" : "yok")}";

    /// <summary>Verilen klasördeki appsettings.json (varsa) ve ortam değişkeninden ayarları okur.</summary>
    public static ClaudeAyarlari Yukle(string klasor) =>
        Yukle(klasor, Environment.GetEnvironmentVariable(OrtamDegiskeni));

    // Testler gerçek ortam değişkenini değiştirmeden önceliği deneyebilsin diye ayrı.
    internal static ClaudeAyarlari Yukle(string klasor, string? ortamAnahtari)
    {
        var bolum = new ConfigurationBuilder()
            .SetBasePath(klasor)
            .AddJsonFile("appsettings.json", optional: true)
            .Build()
            .GetSection("Claude");

        var anahtar = string.IsNullOrWhiteSpace(ortamAnahtari) ? bolum["ApiAnahtari"] : ortamAnahtari;
        // Efor alanı hiç yoksa varsayılan; boş bırakılmışsa gönderilmez.
        var efor = bolum["Efor"] ?? VarsayilanEfor;
        TimeSpan? zamanAsimi = int.TryParse(bolum["ZamanAsimiSaniye"], out var saniye) ? TimeSpan.FromSeconds(saniye) : null;

        return new ClaudeAyarlari(anahtar, bolum["Model"], efor, zamanAsimi);
    }
}
