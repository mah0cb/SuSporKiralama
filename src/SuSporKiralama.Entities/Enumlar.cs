namespace SuSporKiralama.Entities;

// Veritabanında sayı değil metin (string) olarak saklanırlar; bkz. DbContext.ConfigureConventions.

public enum Rol { Admin, Personel }

public enum DeneyimSeviyesi { Baslangic, Orta, Ileri }

public enum EkipmanDurumu { Musait, Kirada, Bakimda, HizmetDisi }

public enum SupBoardTipi { Sisme, Sert }

public enum Beden { S, M, L, XL }

public enum KiralamaDurumu { Aktif, Tamamlandi, IptalEdildi }

public enum OdemeTipi { Nakit, KrediKarti, Havale }
