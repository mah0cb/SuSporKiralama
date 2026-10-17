using System.Globalization;
using DevExpress.Utils.Filtering.Internal;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraLayout.Localization;

namespace SuSporKiralama.UI.Yerellestirme;

/// <summary>
/// Uygulamayı Türkçe çalıştırır: kültür tr-TR olur (tarih, saat, ondalık ayırıcı ve ₺ biçimleri)
/// ve DevExpress'in kendi İngilizce metinleri Türkçe localizer'larla değiştirilir.
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

        // DevExpress kontrolleri kendi metinlerini (buton, menü, ipucu) Active localizer'dan okur.
        // Kontrol oluşturulmadan önce atanmalı; sonradan atanırsa açık formlar eski metni gösterir.
        Localizer.Active = new TurkceEditorLocalizer();
        GridLocalizer.Active = new TurkceGridLocalizer();
        BarLocalizer.Active = new TurkceBarLocalizer();
        LayoutLocalizer.Active = new TurkceLayoutLocalizer();
        FilteringLocalizer.Active = new TurkceFiltreLocalizer();
        FilterUIElementLocalizer.Active = new TurkceFiltreArayuzuLocalizer();
    }
}
