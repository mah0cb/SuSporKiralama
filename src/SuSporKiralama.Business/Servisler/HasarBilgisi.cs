namespace SuSporKiralama.Business.Servisler;

/// <summary>İade sırasında bildirilen hasar: hangi kiralama satırı, açıklama ve bedel.</summary>
public record HasarBilgisi(int KiralamaDetayId, string Aciklama, decimal Bedel);
