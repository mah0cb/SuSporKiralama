using System.IO;
using System.Text.Json;
using DevExpress.LookAndFeel;

namespace SuSporKiralama.UI.Altyapi;

/// <summary>
/// Seçilen DevExpress temasını (ve SVG paletini) kullanıcının AppData klasöründe hatırlar:
/// %AppData%\SuSporKiralama\tema.json. Dosya kullanıcıya özeldir, repoya girmez.
/// Varsayılan tema WXI. Dosya okunamazsa/yazılamazsa uygulama varsayılanla çalışmaya devam eder.
/// </summary>
public static class TemaAyari
{
    public const string VarsayilanTema = "WXI";

    private static readonly string Dosya = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SuSporKiralama", "tema.json");

    private sealed record Tema(string Ad, string? Palet);

    /// <summary>Kayıtlı temayı uygular ve bundan sonraki tema değişikliklerini kaydetmeye başlar.</summary>
    public static void Uygula()
    {
        var tema = Oku() ?? new Tema(VarsayilanTema, null);
        if (string.IsNullOrEmpty(tema.Palet))
            UserLookAndFeel.Default.SetSkinStyle(tema.Ad);
        else
            UserLookAndFeel.Default.SetSkinStyle(tema.Ad, tema.Palet);

        // Ribbon'daki tema galerisinden yapılan her seçim burada yakalanır.
        UserLookAndFeel.Default.StyleChanged += (_, _) => Kaydet();
    }

    private static Tema? Oku()
    {
        try
        {
            return File.Exists(Dosya) ? JsonSerializer.Deserialize<Tema>(File.ReadAllText(Dosya)) : null;
        }
        catch (Exception)
        {
            return null; // bozuk dosya: varsayılan tema
        }
    }

    private static void Kaydet()
    {
        try
        {
            var tema = new Tema(UserLookAndFeel.Default.SkinName, UserLookAndFeel.Default.ActiveSvgPaletteName);
            Directory.CreateDirectory(Path.GetDirectoryName(Dosya)!);
            File.WriteAllText(Dosya, JsonSerializer.Serialize(tema));
        }
        catch (Exception)
        {
            // Tema kaydedilemezse yalnızca bir sonraki açılışta varsayılan tema gelir; işi engellemez.
        }
    }
}
