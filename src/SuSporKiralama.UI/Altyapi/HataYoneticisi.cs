using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.XtraEditors;
using SuSporKiralama.Business.Istisnalar;

namespace SuSporKiralama.UI.Altyapi;

/// <summary>
/// Yakalanmamış tüm hatalar buraya gelir. İş kuralı hataları (IsKuraliException ve türevleri)
/// kullanıcının düzeltebileceği durumlardır: mesajı olduğu gibi uyarı olarak gösterilir.
/// Diğerleri beklenmeyen hatalardır: kullanıcıya genel bir mesaj gösterilir, ayrıntılar
/// logs klasöründeki günlük dosyasına yazılır.
/// </summary>
public static class HataYoneticisi
{
    public const string BeklenmeyenHataMesaji =
        "Beklenmeyen bir hata oluştu. İşlem tamamlanamadı.\n\nAyrıntılar uygulama klasöründeki logs klasörüne kaydedildi.";

    private static readonly string LogKlasoru = Path.Combine(AppContext.BaseDirectory, "logs");
    private static readonly Lock LogKilidi = new();

    /// <summary>Loga yazılmadan önce metinden silinecek gizli değer (Claude API anahtarı).</summary>
    public static string? GizliDeger { get; set; }

    /// <summary>Hata olaylarını bağlar; Main'in başında, herhangi bir form oluşturulmadan çağrılır.</summary>
    public static void Baglan()
    {
        // UI thread'indeki hatalar ThreadException'a gelsin, uygulama çökmesin.
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => Isle(e.Exception);

        // UI dışı thread'lerdeki hatalar: uygulama yine de kapanır, en azından kayıt kalır.
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
                Kaydet(ex);
            MessageBox.Show(BeklenmeyenHataMesaji, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        };

        // Beklenmemiş (await edilmemiş) Task hataları; finalizer thread'inde gelir, ekrana bir şey gösterilmez.
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Kaydet(e.Exception);
            e.SetObserved();
        };
    }

    public static void Isle(Exception hata)
    {
        hata = AsilHata(hata);

        if (hata is IsKuraliException)
        {
            XtraMessageBox.Show(hata.Message, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Kaydet(hata);
        XtraMessageBox.Show(BeklenmeyenHataMesaji, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // Task ve yansıma (reflection) hataları asıl hatayı sarar; tek iç hata varsa o kullanılır.
    private static Exception AsilHata(Exception hata) => hata switch
    {
        AggregateException { InnerExceptions.Count: 1 } a => AsilHata(a.InnerExceptions[0]),
        TargetInvocationException { InnerException: { } ic } => AsilHata(ic),
        _ => hata
    };

    /// <summary>logs/hata-yyyy-MM-dd.log dosyasına zaman, tür, mesaj ve stack trace yazar.</summary>
    public static void Kaydet(Exception hata)
    {
        try
        {
            var metin = new StringBuilder()
                .AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {hata.GetType().FullName}: {hata.Message}")
                .AppendLine(hata.ToString()) // iç hatalar ve stack trace dahil
                .AppendLine(new string('-', 80))
                .ToString();

            if (!string.IsNullOrEmpty(GizliDeger))
                metin = metin.Replace(GizliDeger, "***");

            lock (LogKilidi)
            {
                Directory.CreateDirectory(LogKlasoru);
                File.AppendAllText(Path.Combine(LogKlasoru, $"hata-{DateTime.Now:yyyy-MM-dd}.log"), metin);
            }
        }
        catch (Exception)
        {
            // Log yazılamıyorsa (disk dolu, yetki yok) hata işleyicisi yeni bir hata üretmesin.
        }
    }
}
