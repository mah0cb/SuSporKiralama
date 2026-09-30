namespace SuSporKiralama.Business.Istisnalar;

/// <summary>Geçersiz girdi (boş alan, yanlış format, negatif fiyat vb.).</summary>
public class DogrulamaException : IsKuraliException
{
    public DogrulamaException(string mesaj) : base(mesaj) { }

    /// <summary>
    /// Entity'lerin fırlattığı ArgumentException'ı sarar; orijinal hata InnerException'da korunur.
    /// Entity'ler parametre adı vermediği için mesaj kullanıcıya olduğu gibi gösterilebilir.
    /// </summary>
    public DogrulamaException(ArgumentException icHata) : base(icHata.Message, icHata) { }
}
