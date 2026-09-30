namespace SuSporKiralama.Business.Istisnalar;

/// <summary>Kayda bağlı başka kayıtlar (ör. kiralama geçmişi) olduğu için silinemiyor.</summary>
public class IliskiliKayitVarException(string mesaj) : IsKuraliException(mesaj);
