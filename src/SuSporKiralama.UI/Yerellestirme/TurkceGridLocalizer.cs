using DevExpress.XtraGrid.Localization;

namespace SuSporKiralama.UI.Yerellestirme;

/// <summary>
/// GridControl'ün kendi metinleri (grup paneli, arama paneli, sütun menüsü, filtre pencereleri) bu
/// localizer'dan okunur. Çevrilmeyen ID'ler base ile İngilizce varsayılana düşer.
/// </summary>
internal sealed class TurkceGridLocalizer : GridLocalizer
{
    public override string GetLocalizedString(GridStringId id) => id switch
    {
        GridStringId.GridGroupPanelText => "Gruplamak için bir sütun başlığını buraya sürükleyin",

        // Arama paneli (Ctrl+F)
        GridStringId.FindControlFindButton => "Bul",
        GridStringId.FindControlClearButton => "Temizle",
        GridStringId.FindControlNextButton => "Sonraki",
        GridStringId.FindControlPrevButton => "Önceki",
        GridStringId.FindNullPrompt => "Aramak için yazın...",

        // Sütun başlığı sağ tık menüsü
        GridStringId.MenuColumnSortAscending => "Artan Sırala",
        GridStringId.MenuColumnSortDescending => "Azalan Sırala",
        GridStringId.MenuColumnClearSorting => "Sıralamayı Kaldır",
        GridStringId.MenuColumnClearAllSorting => "Tüm Sıralamaları Kaldır",
        GridStringId.MenuColumnGroup => "Bu Sütuna Göre Grupla",
        GridStringId.MenuColumnUnGroup => "Gruplamayı Kaldır",
        GridStringId.MenuColumnGroupBox => "Grup Panelini Göster",
        GridStringId.MenuColumnShowColumn => "Sütunu Göster",
        GridStringId.MenuColumnRemoveColumn => "Sütunu Gizle",
        GridStringId.MenuColumnColumnCustomization => "Sütun Seçici",
        GridStringId.MenuColumnBestFit => "En Uygun Genişlik",
        GridStringId.MenuColumnBestFitAllColumns => "En Uygun Genişlik (Tüm Sütunlar)",
        GridStringId.MenuColumnFilterEditor => "Filtre Düzenleyici...",
        GridStringId.MenuColumnFilter => "Filtrele",
        GridStringId.MenuColumnClearFilter => "Filtreyi Temizle",
        GridStringId.MenuColumnShowAutoFilter => "Otomatik Filtre Satırını Göster",
        GridStringId.MenuColumnAutoFilterRowShow => "Otomatik Filtre Satırını Göster",
        GridStringId.MenuColumnAutoFilterRowHide => "Otomatik Filtre Satırını Gizle",
        GridStringId.MenuColumnFindFilterShow => "Arama Panelini Göster",
        GridStringId.MenuColumnFindFilterHide => "Arama Panelini Gizle",
        GridStringId.MenuColumnConditionalFormatting => "Koşullu Biçimlendirme",
        GridStringId.MenuColumnExpressionEditor => "İfade Düzenleyici...",
        GridStringId.MenuColumnGroupSummaryEditor => "Grup Özeti Düzenleyici...",

        // Grup paneli ve grup satırı menüsü
        GridStringId.MenuGroupPanelFullExpand => "Tümünü Genişlet",
        GridStringId.MenuGroupPanelFullCollapse => "Tümünü Daralt",
        GridStringId.MenuGroupPanelClearGrouping => "Gruplamayı Temizle",
        GridStringId.MenuGroupPanelShow => "Grup Panelini Göster",
        GridStringId.MenuGroupPanelHide => "Grup Panelini Gizle",
        GridStringId.MenuGroupRowExpand => "Genişlet",
        GridStringId.MenuGroupRowCollapse => "Daralt",

        // Sütun seçici penceresi
        GridStringId.CustomizationCaption => "Sütun Seçici",
        GridStringId.CustomizationColumns => "Sütunlar",
        GridStringId.CustomizationFormColumnHint => "Gizlemek için sütunları buraya sürükleyin",

        // Klasik filtre açılırı ve özel filtre penceresi
        GridStringId.PopupFilterAll => "(Tümü)",
        GridStringId.PopupFilterCustom => "(Özel)",
        GridStringId.PopupFilterBlanks => "(Boşlar)",
        GridStringId.PopupFilterNonBlanks => "(Boş olmayanlar)",
        GridStringId.CustomFilterDialogFormCaption => "Özel Filtre",
        GridStringId.CustomFilterDialogCaption => "Şu satırları göster:",
        GridStringId.CustomFilterDialogRadioAnd => "Ve",
        GridStringId.CustomFilterDialogRadioOr => "Veya",
        GridStringId.CustomFilterDialogOkButton => "Tamam",
        GridStringId.CustomFilterDialogCancelButton => "İptal",
        GridStringId.CustomFilterDialogClearFilter => "Filtreyi Temizle",

        // Filtre düzenleyici penceresi ve alttaki filtre paneli
        GridStringId.FilterBuilderCaption => "Filtre Düzenleyici",
        GridStringId.FilterBuilderOkButton => "Tamam",
        GridStringId.FilterBuilderCancelButton => "İptal",
        GridStringId.FilterBuilderApplyButton => "Uygula",
        GridStringId.FilterPanelCustomizeButton => "Filtreyi Düzenle",

        GridStringId.CheckboxSelectorColumnCaption => "Seç",

        _ => base.GetLocalizedString(id)
    };
}
