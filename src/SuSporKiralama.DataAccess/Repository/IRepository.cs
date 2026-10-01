using System.Linq.Expressions;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.DataAccess.Repository;

/// <summary>
/// Tüm entity'ler için ortak veri erişim sözleşmesi (generic arayüz).
/// "where T : BaseEntity" kısıtı sayesinde sadece entity sınıfları kullanılabilir
/// ve her T'nin bir Id'si olduğu garanti edilir.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    List<T> GetAll();
    T? GetById(int id);

    /// <summary>
    /// Koşula uyan kayıtlar, ör. Find(m => m.Telefon == "555...").
    /// İlişkili veriler yol olarak verilebilir: Find(k => k.Id == id, "Detaylar.Ekipman").
    /// </summary>
    List<T> Find(Expression<Func<T, bool>> kosul, params string[] iliskiler);

    /// <summary>
    /// Salt okunur (takip edilmeyen) sorgu. Gruplama, toplam ve sayım gibi rapor sorgularının
    /// tamamı veritabanında (SQL olarak) çalışsın diye; sonuç nesneleri değiştirilip kaydedilmez.
    /// </summary>
    IQueryable<T> Query();

    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);

    /// <summary>
    /// Nesnedeki kaydedilmemiş değişiklikleri atar, değerleri veritabanından yeniden okur.
    /// Kural ihlali nedeniyle reddedilen bir güncelleme, sonraki bir SaveChanges ile
    /// yanlışlıkla veritabanına yazılmasın diye kullanılır.
    /// </summary>
    void Reload(T entity);

    /// <summary>Bekleyen değişiklikleri veritabanına yazar; etkilenen kayıt sayısını döner.</summary>
    int SaveChanges();
}
