namespace SuSporKiralama.UI.Altyapi;

/// <summary>
/// Açılışta bağlantı dizesinden okunan, ekranda gösterilen bilgiler (durum çubuğu).
/// Formlar DbContext'e erişmesin diye ayrı bir nesne olarak verilir.
/// </summary>
public record UygulamaBilgisi(string VeritabaniAdi);
