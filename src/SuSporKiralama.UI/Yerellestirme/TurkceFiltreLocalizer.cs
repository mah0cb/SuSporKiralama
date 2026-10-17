using DevExpress.Utils.Filtering.Internal;

namespace SuSporKiralama.UI.Yerellestirme;

/// <summary>
/// Excel tarzı sütun filtresindeki değer listesi metinleri ("Tümünü Seç", "Boşlar") bu localizer'dan
/// okunur (sınıf DevExpress'in Internal ad alanında ama public). Çevrilmeyen ID'ler base ile İngilizce varsayılana düşer.
/// </summary>
internal sealed class TurkceFiltreLocalizer : FilteringLocalizer
{
    public override string GetLocalizedString(FilteringLocalizerStringId id) => id switch
    {
        FilteringLocalizerStringId.SelectAllName => "(Tümünü Seç)",
        FilteringLocalizerStringId.BlanksName => "(Boşlar)",
        FilteringLocalizerStringId.NonBlanksName => "(Boş olmayanlar)",
        FilteringLocalizerStringId.NullName => "(Boş)",
        FilteringLocalizerStringId.EmptyName => "(Boş)",
        FilteringLocalizerStringId.TrueName => "Evet",
        FilteringLocalizerStringId.FalseName => "Hayır",
        FilteringLocalizerStringId.FromName => "Başlangıç",
        FilteringLocalizerStringId.ToName => "Bitiş",
        FilteringLocalizerStringId.MenuSelectAllName => "Tümünü Seç",
        FilteringLocalizerStringId.MenuClearSelectionName => "Seçimi Temizle",
        FilteringLocalizerStringId.MenuInvertSelectionName => "Seçimi Tersine Çevir",
        FilteringLocalizerStringId.MenuResetFilter => "Filtreyi Sıfırla",
        FilteringLocalizerStringId.MenuResetAllFilters => "Tüm Filtreleri Sıfırla",

        _ => base.GetLocalizedString(id)
    };
}
