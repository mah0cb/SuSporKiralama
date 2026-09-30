using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Business.Servisler.Soyut;

/// <summary>
/// Tüm entity servislerinin ortak sözleşmesi (generic arayüz). UI yalnızca bu arayüzleri
/// bilir; hangi sınıfın çalıştığını bilmek zorunda değildir (soyutlama).
/// </summary>
public interface ICrudServisi<T> where T : BaseEntity
{
    List<T> TumunuGetir();

    /// <summary>Kayıt yoksa KayitBulunamadiException fırlatır.</summary>
    T IdIleGetir(int id);

    void Ekle(T entity);
    void Guncelle(T entity);
    void Sil(int id);
}
