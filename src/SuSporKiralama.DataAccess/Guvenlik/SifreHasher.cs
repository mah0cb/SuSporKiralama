using System.Security.Cryptography;

namespace SuSporKiralama.DataAccess.Guvenlik;

/// <summary>
/// Şifreleri PBKDF2 (SHA-256) ile özetler. Her şifre için rastgele bir salt üretilir;
/// böylece aynı şifreye sahip iki kullanıcının hash'i bile farklı olur.
/// Saklama biçimi: "iterasyon.salt.hash" (salt ve hash Base64).
/// Doğrulama metodu giriş ekranıyla birlikte (4. aşama) eklenecek.
/// </summary>
public static class SifreHasher
{
    private const int Iterasyon = 100_000;
    private const int SaltBoyutu = 16; // byte
    private const int HashBoyutu = 32; // byte

    public static string Hashle(string sifre)
    {
        if (string.IsNullOrEmpty(sifre))
            throw new ArgumentException("Şifre boş olamaz.", nameof(sifre));

        var salt = RandomNumberGenerator.GetBytes(SaltBoyutu);
        var hash = Rfc2898DeriveBytes.Pbkdf2(sifre, salt, Iterasyon, HashAlgorithmName.SHA256, HashBoyutu);
        return $"{Iterasyon}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Değer bu sınıfın ürettiği "iterasyon.salt.hash" biçiminde mi? Düz şifrenin
    /// yanlışlıkla hash yerine kaydedilmesini önlemek için kullanılır.
    /// </summary>
    public static bool GecerliHashMi(string? deger)
    {
        var parcalar = deger?.Split('.');
        if (parcalar is not { Length: 3 })
            return false;

        return int.TryParse(parcalar[0], out var iterasyon) && iterasyon > 0
            && Base64Uzunlugu(parcalar[1]) == SaltBoyutu
            && Base64Uzunlugu(parcalar[2]) == HashBoyutu;
    }

    // Base64 metnin çözülmüş byte sayısı; geçersizse -1.
    private static int Base64Uzunlugu(string metin)
    {
        var tampon = new byte[metin.Length];
        return Convert.TryFromBase64String(metin, tampon, out var uzunluk) ? uzunluk : -1;
    }
}
