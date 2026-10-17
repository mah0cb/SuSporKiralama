using DevExpress.XtraLayout.Localization;

namespace SuSporKiralama.UI.Yerellestirme;

/// <summary>
/// LayoutControl'ün çalışma anı sağ tık menüsü ("Düzeni Özelleştir") ve özelleştirme penceresi metinleri
/// bu localizer'dan okunur. Çevrilmeyen ID'ler base ile İngilizce varsayılana düşer.
/// </summary>
internal sealed class TurkceLayoutLocalizer : LayoutLocalizer
{
    public override string GetLocalizedString(LayoutStringId id) => id switch
    {
        LayoutStringId.ShowCustomizationFormMenuText => "Düzeni Özelleştir",
        LayoutStringId.HideCustomizationFormMenuText => "Özelleştirmeyi Bitir",
        LayoutStringId.CustomizationFormTitle => "Düzeni Özelleştir",
        LayoutStringId.HiddenItemsPageTitle => "Gizli Öğeler",
        LayoutStringId.TreeViewPageTitle => "Düzen Ağacı",
        LayoutStringId.HideItemMenutext => "Öğeyi Gizle",
        LayoutStringId.ResetLayoutMenuText => "Düzeni Sıfırla",
        LayoutStringId.RenameMenuText => "Yeniden Adlandır",
        LayoutStringId.ShowTextMenuItem => "Metni Göster",
        LayoutStringId.HideTextMenuItem => "Metni Gizle",
        LayoutStringId.TextPositionMenuText => "Metin Konumu",
        LayoutStringId.TextPositionLeftMenuText => "Sol",
        LayoutStringId.TextPositionRightMenuText => "Sağ",
        LayoutStringId.TextPositionTopMenuText => "Üst",
        LayoutStringId.TextPositionBottomMenuText => "Alt",
        LayoutStringId.LockMenuGroup => "Boyut Kilidi",
        LayoutStringId.GroupItemsMenuText => "Grupla",
        LayoutStringId.UnGroupItemsMenuText => "Gruplamayı Kaldır",
        LayoutStringId.LayoutResetConfirmationText => "Düzen varsayılan haline döndürülsün mü?",
        LayoutStringId.LayoutResetConfirmationDialogCaption => "Düzeni Sıfırla",

        _ => base.GetLocalizedString(id)
    };
}
