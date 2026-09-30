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

    public List<T> Find(Expression<Func<T, bool>> kosul) => Set.Where(kosul).ToList();

    public void Add(T entity) => Set.Add(entity);

    public void Update(T entity) => Set.Update(entity);

    public void Delete(T entity) => Set.Remove(entity);

    public int SaveChanges() => Context.SaveChanges();
}
