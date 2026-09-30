namespace SuSporKiralama.Business.Istisnalar;

/// <summary>
/// Girdi geçerli ama mevcut durumda işleme izin verilmiyor
/// (ör. son aktif admin pasifleştirilemez, ekipman elle Kirada yapılamaz).
/// </summary>
public class IslemYapilamazException(string mesaj) : IsKuraliException(mesaj);
