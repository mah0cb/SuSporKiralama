namespace SuSporKiralama.Business.Istisnalar;

/// <summary>Benzersiz olması gereken bir alan (telefon, ekipman kodu, kullanıcı adı) zaten kullanılıyor.</summary>
public class BenzersizlikIhlaliException(string alanAdi, string deger)
    : IsKuraliException($"{alanAdi} \"{deger}\" zaten kullanılıyor.")
{
    public string AlanAdi { get; } = alanAdi;
    public string Deger { get; } = deger;
}
