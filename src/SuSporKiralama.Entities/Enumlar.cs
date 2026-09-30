namespace SuSporKiralama.Entities;

// Veritabanında sayı değil metin (string) olarak saklanırlar; bkz. DbContext.ConfigureConventions.

public enum Rol { Admin, Personel }

public enum DeneyimSeviyesi { Baslangic, Orta, Ileri }

public enum EkipmanDurumu { Musait, Kirada, Bakimda, HizmetDisi }

public enum SupBoardTipi { Sisme, Sert }

public enum Beden { S, M, L, XL }

public enum KiralamaDurumu { Rezerve, Aktif, Tamamlandi, IptalEdildi }

// Alinmadi: rezervasyon/iptal; Alindi: kiralama aktif; iade sonrası hasar mahsubuna göre son üç durumdan biri.
public enum DepozitoDurumu { Alinmadi, Alindi, IadeEdildi, KismenIadeEdildi, MahsupEdildi }

public enum OdemeTipi { Nakit, KrediKarti, Havale }
