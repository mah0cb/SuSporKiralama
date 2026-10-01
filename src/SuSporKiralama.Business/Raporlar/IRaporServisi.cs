namespace SuSporKiralama.Business.Raporlar;

/// <summary>
/// Dashboard ve rapor sorguları. Tümü RaporGoruntuleme yetkisi ister.
/// Gelir = ödeme tarihine göre alınan ödemeler (Odemeler tablosu).
/// </summary>
public interface IRaporServisi
{
    /// <summary>[baslangic, bitis] aralığındaki her gün için gelir (iki uç dahil); ödeme olmayan günler 0.</summary>
    List<GunlukGelir> GunlukGelirler(DateOnly baslangic, DateOnly bitis);

    /// <summary>Yılın 12 ayı için gelir; ödeme olmayan aylar 0.</summary>
    List<AylikGelir> AylikGelirler(int yil);

    /// <summary>
    /// Başlangıcı [baslangic, bitis) içinde olan teslim edilmiş (Aktif/Tamamlandı) kiralamalara göre
    /// en çok kiralanan ilk "adet" ekipman. Sıra: kiralanma sayısı, sonra toplam süre.
    /// </summary>
    List<EkipmanKiralamaIstatistigi> EnCokKiralananlar(DateTime baslangic, DateTime bitis, int adet);

    /// <summary>
    /// [baslangic, bitis] günlerinde ekipman türü başına doluluk: kiralanan saat /
    /// (hizmet dışı olmayan ekipman sayısı × çalışma saati). Yalnızca çalışma saatleri içi sayılır.
    /// </summary>
    List<DolulukOrani> DolulukOranlari(DateOnly baslangic, DateOnly bitis);

    /// <summary>Ana ekran özeti; "bugün" ve "şimdi" TimeProvider'dan alınır.</summary>
    DashboardOzeti DashboardOzeti();
}
