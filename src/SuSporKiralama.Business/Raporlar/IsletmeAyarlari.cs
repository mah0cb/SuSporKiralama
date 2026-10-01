namespace SuSporKiralama.Business.Raporlar;

/// <summary>
/// İşletmeye özgü ayarlar; doluluk oranı "açık olunan saat" üzerinden hesaplandığı için gerekir.
/// Koda gömülmez: RaporServisi'ne constructor'dan verilir (UI ileride appsettings'ten okuyup verebilir).
/// </summary>
public record IsletmeAyarlari(TimeOnly AcilisSaati, TimeOnly KapanisSaati)
{
    public static IsletmeAyarlari Varsayilan { get; } = new(new TimeOnly(9, 0), new TimeOnly(19, 0));

    public TimeSpan GunlukAcikSure => KapanisSaati - AcilisSaati;
}
