namespace SuSporKiralama.Entities;

/// <summary>
/// Entity setter'larında tekrar eden doğrulamalar için küçük yardımcı.
/// Kural bozulursa anlamlı Türkçe mesajlı ArgumentException fırlatır (kapsülleme).
/// </summary>
internal static class Dogrula
{
    public static string BosOlamaz(string? deger, string alanAdi)
    {
        if (string.IsNullOrWhiteSpace(deger))
            throw new ArgumentException($"{alanAdi} boş olamaz.", alanAdi);
        return deger.Trim();
    }

    public static decimal NegatifOlamaz(decimal deger, string alanAdi)
    {
        if (deger < 0)
            throw new ArgumentException($"{alanAdi} negatif olamaz.", alanAdi);
        return deger;
    }

    public static int PozitifOlmali(int deger, string alanAdi)
    {
        if (deger <= 0)
            throw new ArgumentException($"{alanAdi} sıfırdan büyük olmalıdır.", alanAdi);
        return deger;
    }
}
