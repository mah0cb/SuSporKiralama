using DevExpress.XtraBars.Localization;

namespace SuSporKiralama.UI.Yerellestirme;

/// <summary>
/// Ribbon ve araç çubuklarının metinleri (Ribbon sağ tık menüsü, Hızlı Erişim Araç Çubuğu, tema galerisi)
/// bu localizer'dan okunur. Çevrilmeyen ID'ler base ile İngilizce varsayılana düşer.
/// </summary>
internal sealed class TurkceBarLocalizer : BarLocalizer
{
    public override string GetLocalizedString(BarString id) => id switch
    {
        // Ribbon'a sağ tıklayınca açılan menü
        BarString.RibbonToolbarAbove => "Hızlı Erişim Araç Çubuğunu Şeridin Üstünde Göster",
        BarString.RibbonToolbarBelow => "Hızlı Erişim Araç Çubuğunu Şeridin Altında Göster",
        BarString.RibbonToolbarAdd => "Hızlı Erişim Araç Çubuğuna Ekle",
        BarString.RibbonToolbarRemove => "Hızlı Erişim Araç Çubuğundan Kaldır",
        BarString.RibbonToolbarMinimizeRibbon => "Şeridi Küçült",
        BarString.ShowQuickAccessToolbarItemCaption => "Hızlı Erişim Araç Çubuğunu Göster",
        BarString.HideQuickAccessToolbarItemCaption => "Hızlı Erişim Araç Çubuğunu Gizle",
        BarString.CustomizeQuickAccessToolbar => "Hızlı Erişim Araç Çubuğunu Özelleştir",
        BarString.CustomizeRibbon => "Şeridi Özelleştir...",
        BarString.MoreCommands => "Diğer Komutlar...",

        // Ribbon görünüm seçenekleri
        BarString.RibbonMinimizedMenuItemCaption => "Yalnızca Sekmeleri Göster",
        BarString.RibbonExpandedMenuItemCaption => "Her Zaman Şeridi Göster",
        BarString.RibbonFullScreenMenuItemCaption => "Şeridi Otomatik Gizle",
        BarString.ExpandRibbonSuperTipHeader => "Şeridi Genişlet",
        BarString.CollapseRibbonSuperTipHeader => "Şeridi Daralt",
        BarString.ClassicRibbonLayoutItemCaption => "Klasik Şerit",
        BarString.SimplifiedRibbonLayoutItemCaption => "Basitleştirilmiş Şerit",
        BarString.RibbonSearchItemNullText => "Komut ara...",
        BarString.RibbonSearchItemNoMatchesFound => "Sonuç bulunamadı",

        // Galeri (tema galerisi)
        BarString.RibbonGalleryFilter => "Tüm Gruplar",
        BarString.RibbonGalleryFilterNone => "Hiçbiri",
        BarString.RibbonAllPages => "Tüm Sayfalar",

        // Pencere butonlarının ipuçları
        BarString.CloseButton => "Kapat",
        BarString.MinimizeButton => "Simge Durumuna Küçült",
        BarString.MaximizeButton => "Ekranı Kapla",
        BarString.RestoreButton => "Önceki Boyut",
        BarString.HelpButton => "Yardım",

        _ => base.GetLocalizedString(id)
    };
}
