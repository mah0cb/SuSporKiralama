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

    /// <summary>Ana ekran özeti; "bugün" ve "şimdi" TimeProvider'dan alınır.</summary>
    DashboardOzeti DashboardOzeti();
}
