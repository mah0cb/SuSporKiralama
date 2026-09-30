namespace SuSporKiralama.Business.Istisnalar;

/// <summary>
/// Girdi geçerli ama mevcut durumda işleme izin verilmiyor
/// (ör. son aktif admin pasifleştirilemez, ekipman elle Kirada yapılamaz, aktif kiralama iptal edilemez).
/// </summary>
public class IslemYapilamazException(string mesaj, Exception? icHata = null) : IsKuraliException(mesaj, icHata);
