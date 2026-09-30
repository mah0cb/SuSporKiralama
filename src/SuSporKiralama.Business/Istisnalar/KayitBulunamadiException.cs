namespace SuSporKiralama.Business.Istisnalar;

/// <summary>Id ile aranan kayıt veritabanında yok.</summary>
public class KayitBulunamadiException(string entityAdi, int id)
    : IsKuraliException($"{entityAdi} bulunamadı (Id: {id}).")
{
    public string EntityAdi { get; } = entityAdi;
    public int Id { get; } = id;
}
