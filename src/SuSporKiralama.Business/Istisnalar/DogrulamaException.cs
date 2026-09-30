namespace SuSporKiralama.Business.Istisnalar;

/// <summary>Geçersiz girdi (boş alan, yanlış format, negatif fiyat vb.).</summary>
public class DogrulamaException : IsKuraliException
{
    public DogrulamaException(string mesaj) : base(mesaj) { }

    /// <summary>
    /// Entity'lerin fırlattığı ArgumentException'ı sarar. Orijinal hata InnerException'da korunur;
    /// .NET'in mesaja eklediği " (Parameter 'x')" eki kullanıcıya gösterilmesin diye çıkarılır.
    /// </summary>
    public DogrulamaException(ArgumentException icHata) : base(TemizMesaj(icHata), icHata) { }

    private static string TemizMesaj(ArgumentException hata) =>
        hata.ParamName is null
            ? hata.Message
            : hata.Message.Replace($" (Parameter '{hata.ParamName}')", "");
}
