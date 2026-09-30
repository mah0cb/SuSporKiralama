using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.DataAccess.Repository;

/// <summary>
/// IRepository&lt;T&gt;'nin EF Core implementasyonu. Tek bir generic sınıf,
/// tüm entity'ler için çalışır: new EfRepository&lt;Musteri&gt;(context) gibi.
/// </summary>
public class EfRepository<T>(SuSporKiralamaDbContext context) : IRepository<T> where T : BaseEntity
{
    // Alt sınıflar (ileride özel repository gerekirse) context'e erişebilsin diye protected.
    protected readonly SuSporKiralamaDbContext Context = context;
    protected DbSet<T> Set => Context.Set<T>();

    public List<T> GetAll() => Set.ToList();

    public T? GetById(int id) => Set.Find(id);

    // Her ilişki yolu için Include eklenir (EF Core noktalı yolları, ör. "Detaylar.Ekipman", destekler).
    public List<T> Find(Expression<Func<T, bool>> kosul, params string[] iliskiler) =>
        iliskiler.Aggregate(Set.AsQueryable(), (sorgu, yol) => sorgu.Include(yol))
            .Where(kosul)
            .ToList();

    public void Add(T entity) => Set.Add(entity);

    public void Update(T entity) => Set.Update(entity);

    public void Delete(T entity) => Set.Remove(entity);

    public void Reload(T entity)
    {
        // Context'in takip etmediği (Detached) nesnede geri alınacak değişiklik yoktur.
        var entry = Context.Entry(entity);
        if (entry.State != EntityState.Detached)
            entry.Reload();
    }

    public int SaveChanges() => Context.SaveChanges();
}
