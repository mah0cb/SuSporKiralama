namespace SuSporKiralama.Business.Istisnalar;

/// <summary>
/// Tüm iş kuralı hatalarının soyut atası. Mesajı kullanıcıya doğrudan gösterilebilecek
/// Türkçe bir metindir. UI bu türü yakalayıp mesajı gösterir; diğer (beklenmeyen)
/// exception'lar genel hata yönetimine kalır.
/// Soyut olduğu için doğrudan fırlatılamaz; hatanın türünü belirten alt sınıflar kullanılır.
/// </summary>
public abstract class IsKuraliException : Exception
{
    protected IsKuraliException(string mesaj, Exception? icHata = null) : base(mesaj, icHata) { }
}
