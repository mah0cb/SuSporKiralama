namespace SuSporKiralama.Business.Istisnalar;

/// <summary>
/// Giriş yapılamadı: hatalı kullanıcı adı/şifre, pasif hesap veya çok fazla hatalı deneme.
/// </summary>
public class GirisBasarisizException(string mesaj) : IsKuraliException(mesaj);
