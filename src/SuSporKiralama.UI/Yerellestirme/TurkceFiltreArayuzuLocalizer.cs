using DevExpress.Utils.Filtering.Internal;

namespace SuSporKiralama.UI.Yerellestirme;

/// <summary>
/// Excel tarzı sütun filtresinin sekmeleri (Değerler / Filtreler), butonları ve koşul adları bu
/// localizer'dan okunur. Çevrilmeyen ID'ler base ile İngilizce varsayılana düşer.
/// </summary>
internal sealed class TurkceFiltreArayuzuLocalizer : FilterUIElementLocalizer
{
    public override string GetLocalizedString(FilterUIElementLocalizerStringId id) => id switch
    {
        FilterUIElementLocalizerStringId.FilteringUITabValues => "Değerler",
        FilterUIElementLocalizerStringId.FilteringUITabGroups => "Filtreler",
        FilterUIElementLocalizerStringId.FilteringUIClearFilter => "Filtreyi Temizle",
        FilterUIElementLocalizerStringId.FilteringUIClose => "Kapat",
        FilterUIElementLocalizerStringId.FilteringUIMoreButtonCaption => "Daha fazla",
        FilterUIElementLocalizerStringId.FilteringUIFewerButtonCaption => "Daha az",
        FilterUIElementLocalizerStringId.CustomUIValueLabel => "Değer",
        FilterUIElementLocalizerStringId.CustomUITypeLabel => "Tür",

        // "Filtreler" sekmesindeki koşullar
        FilterUIElementLocalizerStringId.CustomUIFilterEqualsName => "Eşittir...",
        FilterUIElementLocalizerStringId.CustomUIFilterDoesNotEqualName => "Eşit değildir...",
        FilterUIElementLocalizerStringId.CustomUIFilterBetweenName => "Arasında...",
        FilterUIElementLocalizerStringId.CustomUIFilterIsNullName => "Boştur",
        FilterUIElementLocalizerStringId.CustomUIFilterIsNotNullName => "Boş değildir",
        FilterUIElementLocalizerStringId.CustomUIFilterGreaterThanName => "Büyüktür...",
        FilterUIElementLocalizerStringId.CustomUIFilterGreaterThanOrEqualToName => "Büyük veya eşittir...",
        FilterUIElementLocalizerStringId.CustomUIFilterLessThanName => "Küçüktür...",
        FilterUIElementLocalizerStringId.CustomUIFilterLessThanOrEqualToName => "Küçük veya eşittir...",
        FilterUIElementLocalizerStringId.CustomUIFilterTopNName => "İlk N...",
        FilterUIElementLocalizerStringId.CustomUIFilterBottomNName => "Son N...",
        FilterUIElementLocalizerStringId.CustomUIFilterAboveAverageName => "Ortalamanın üstünde",
        FilterUIElementLocalizerStringId.CustomUIFilterBelowAverageName => "Ortalamanın altında",
        FilterUIElementLocalizerStringId.CustomUIFilterBeginsWithName => "İle başlar...",
        FilterUIElementLocalizerStringId.CustomUIFilterEndsWithName => "İle biter...",
        FilterUIElementLocalizerStringId.CustomUIFilterContainsName => "İçerir...",
        FilterUIElementLocalizerStringId.CustomUIFilterDoesNotContainName => "İçermez...",
        FilterUIElementLocalizerStringId.CustomUIFilterIsBlankName => "Boştur",

        // Koşul editörlerinin boş istemleri
        FilterUIElementLocalizerStringId.CustomUINullValuePromptEnterAValue => "Değer girin",
        FilterUIElementLocalizerStringId.CustomUINullValuePromptSelectAValue => "Değer seçin",
        FilterUIElementLocalizerStringId.CustomUINullValuePromptEnterADate => "Tarih girin",
        FilterUIElementLocalizerStringId.CustomUINullValuePromptSelectADate => "Tarih seçin",
        FilterUIElementLocalizerStringId.CustomUINullValuePromptChooseOne => "Birini seçin",
        FilterUIElementLocalizerStringId.CustomUINullValuePromptSearchControl => "Ara",

        _ => base.GetLocalizedString(id)
    };
}
