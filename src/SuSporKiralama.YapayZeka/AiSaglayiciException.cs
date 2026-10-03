namespace SuSporKiralama.YapayZeka;

/// <summary>
/// Yapay zeka sağlayıcısına ulaşılamadı ya da yanıtı kullanılamaz (anahtar yok, HTTP hatası,
/// zaman aşımı, şemaya uymayan yanıt). İş kuralı hatası değildir; YedekliOneriSaglayici yakalayıp
/// kural tabanlı öneriye geçer. Mesajlara API anahtarı ve yanıt gövdesi asla yazılmaz.
/// </summary>
public class AiSaglayiciException(string mesaj, Exception? icHata = null) : Exception(mesaj, icHata);
