using System.Globalization;

namespace SuSporKiralama.UI.Yerellestirme;

/// <summary>
/// Uygulamayı Türkçe çalıştırır: kültür tr-TR olur (tarih, saat, ondalık ayırıcı ve ₺ biçimleri).
/// Herhangi bir form veya mesaj kutusu oluşturulmadan önce çağrılmalıdır.
/// </summary>
public static class TurkceArayuz
{
    public static void Uygula()
    {
        // Default...: sonradan açılan iş parçacıkları (ör. zamanlayıcı, async devamı) da aynı kültürü alır.
        var turkce = CultureInfo.GetCultureInfo("tr-TR");
        CultureInfo.DefaultThreadCurrentCulture = turkce;
        CultureInfo.DefaultThreadCurrentUICulture = turkce;
        CultureInfo.CurrentCulture = turkce;
        CultureInfo.CurrentUICulture = turkce;
    }
}
