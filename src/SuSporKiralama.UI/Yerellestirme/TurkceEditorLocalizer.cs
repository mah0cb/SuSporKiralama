using DevExpress.XtraEditors.Controls;

namespace SuSporKiralama.UI.Yerellestirme;

/// <summary>
/// DevExpress editörleri ve XtraMessageBox metinlerini bu localizer'dan okur. Yalnızca kullanıcının
/// gördüğü metinler Türkçe verilir; diğer ID'ler base ile DevExpress'in varsayılanına (İngilizce) düşer.
/// </summary>
internal sealed class TurkceEditorLocalizer : Localizer
{
    public override string GetLocalizedString(StringId id) => id switch
    {
        // Mesaj kutusu butonları
        StringId.XtraMessageBoxOkButtonText => "Tamam",
        StringId.XtraMessageBoxCancelButtonText => "İptal",
        StringId.XtraMessageBoxYesButtonText => "Evet",
        StringId.XtraMessageBoxNoButtonText => "Hayır",
        StringId.XtraMessageBoxAbortButtonText => "Durdur",
        StringId.XtraMessageBoxRetryButtonText => "Yeniden Dene",
        StringId.XtraMessageBoxIgnoreButtonText => "Yoksay",
        StringId.XtraMessageBoxDoNotShowThisMessageAgain => "Bu mesajı bir daha gösterme",
        StringId.Cancel => "İptal",
        StringId.CaptionError => "Hata",
        StringId.InvalidValueText => "Geçersiz değer",

        // Tarih editörü
        StringId.DateEditToday => "Bugün",
        StringId.DateEditClear => "Temizle",

        // Metin kutusunun sağ tık menüsü
        StringId.TextEditMenuUndo => "Geri Al",
        StringId.TextEditMenuCut => "Kes",
        StringId.TextEditMenuCopy => "Kopyala",
        StringId.TextEditMenuPaste => "Yapıştır",
        StringId.TextEditMenuDelete => "Sil",
        StringId.TextEditMenuSelectAll => "Tümünü Seç",

        StringId.DefaultBooleanTrue => "Evet",
        StringId.DefaultBooleanFalse => "Hayır",
        StringId.SearchControlNullValuePrompt => "Aramak için yazın...",

        // Filtre düzenleyici (grid'deki "Filtre Düzenleyici" penceresi)
        StringId.FilterShowAll => "(Tümü)",
        StringId.FilterGroupAnd => "Ve",
        StringId.FilterGroupOr => "Veya",
        StringId.FilterGroupNotAnd => "Ve Değil",
        StringId.FilterGroupNotOr => "Veya Değil",
        StringId.FilterClauseEquals => "Eşittir",
        StringId.FilterClauseDoesNotEqual => "Eşit değildir",
        StringId.FilterClauseGreater => "Büyüktür",
        StringId.FilterClauseGreaterOrEqual => "Büyük veya eşittir",
        StringId.FilterClauseLess => "Küçüktür",
        StringId.FilterClauseLessOrEqual => "Küçük veya eşittir",
        StringId.FilterClauseBetween => "Arasındadır",
        StringId.FilterClauseNotBetween => "Arasında değildir",
        StringId.FilterClauseContains => "İçerir",
        StringId.FilterClauseDoesNotContain => "İçermez",
        StringId.FilterClauseBeginsWith => "İle başlar",
        StringId.FilterClauseEndsWith => "İle biter",
        StringId.FilterClauseLike => "Benzer",
        StringId.FilterClauseNotLike => "Benzer değil",
        StringId.FilterClauseIsNull => "Boştur",
        StringId.FilterClauseIsNotNull => "Boş değildir",
        StringId.FilterClauseIsNullOrEmpty => "Boştur",
        StringId.FilterClauseIsNotNullOrEmpty => "Boş değildir",
        StringId.FilterClauseAnyOf => "Şunlardan biri",
        StringId.FilterClauseNoneOf => "Şunlardan hiçbiri",
        StringId.FilterClauseBetweenAnd => "ve",
        StringId.FilterEmptyEnter => "<değer girin>",
        StringId.FilterEmptyValue => "<boş>",
        StringId.FilterMenuConditionAdd => "Koşul Ekle",
        StringId.FilterMenuGroupAdd => "Grup Ekle",
        StringId.FilterMenuRowRemove => "Grubu Kaldır",
        StringId.FilterMenuClearAll => "Tümünü Temizle",
        StringId.FilterEditorTabVisual => "Görsel",
        StringId.FilterEditorTabText => "Metin",

        // Excel tarzı sütun filtresinin araç çubuğu
        StringId.FilterPopupToolbarShowOnlyAvailableItems => "Yalnızca mevcut değerleri göster",
        StringId.FilterPopupToolbarShowNewValues => "Yeni değerleri göster",
        StringId.FilterPopupToolbarIncrementalSearch => "Yazdıkça ara",
        StringId.FilterPopupToolbarMultiSelection => "Çoklu seçim",
        StringId.FilterPopupToolbarRadioMode => "Tekli seçim",
        StringId.FilterPopupToolbarInvertFilter => "Filtreyi tersine çevir",

        _ => base.GetLocalizedString(id)
    };
}
