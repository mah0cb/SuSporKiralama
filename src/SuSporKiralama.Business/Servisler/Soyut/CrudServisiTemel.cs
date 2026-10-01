using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.DataAccess.Repository;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Business.Servisler.Soyut;

/// <summary>
/// Ortak CRUD akışını tek yerde uygulayan soyut temel sınıf (Template Method deseni).
/// Ekle/Guncelle/Sil'in adımları burada sabittir; alt servisler yalnızca "kanca" (hook)
/// metotlarını override ederek kendi iş kurallarını araya ekler.
/// </summary>
public abstract class CrudServisiTemel<T>(IRepository<T> repository, IYetkiServisi yetki) : ICrudServisi<T> where T : BaseEntity
{
    // Alt servisler kendi sorguları için kullanabilsin diye protected.
    protected readonly IRepository<T> Repository = repository;
    protected readonly IYetkiServisi Yetki = yetki;

    // Hata mesajlarında kullanılan kullanıcı dostu ad (ör. "Müşteri").
    // abstract: her servisin kendi adını vermesi zorunludur, makul bir varsayılan yoktur.
    protected abstract string EntityAdi { get; }

    // Ekle/Guncelle/Sil için gereken yetki. abstract: unutulursa derleme hatası olsun.
    // Okuma metotları (TumunuGetir, IdIleGetir) yetki kontrolüne girmez.
    protected abstract Islem YonetimIslemi { get; }

    // Aşağıdaki public metotlar bilerek virtual DEĞİL: akış (kontrol → kaydet) sabittir,
    // alt sınıflar bu sırayı bozamaz; sadece kancaları değiştirebilir.

    public List<T> TumunuGetir() => Repository.GetAll();

    public T IdIleGetir(int id) =>
        Repository.GetById(id) ?? throw new KayitBulunamadiException(EntityAdi, id);

    public void Ekle(T entity)
    {
        Yetki.YetkiKontrol(YonetimIslemi);
        DogrulamaIle(() => EklemeOncesiKontrol(entity));
        Repository.Add(entity);
        Repository.SaveChanges();
    }

    public void Guncelle(T entity)
    {
        Yetki.YetkiKontrol(YonetimIslemi);
        GuncellemeAkisi(entity);
    }

    public void Sil(int id)
    {
        Yetki.YetkiKontrol(YonetimIslemi);
        var entity = IdIleGetir(id);
        SilmeOncesiKontrol(entity);
        Repository.Delete(entity);
        Repository.SaveChanges();
    }

    /// <summary>
    /// Guncelle'nin yetki kontrolü olmayan akışı. Kendi yetkisini ayrıca kontrol eden özel işlemler
    /// (ör. Personel rolünün de yapabildiği ekipman durum değişikliği) bunu çağırır; genel
    /// Guncelle'yi çağırsalar YonetimIslemi yetkisine takılırlardı.
    /// </summary>
    protected void GuncellemeAkisi(T entity)
    {
        IdIleGetir(entity.Id);
        try
        {
            DogrulamaIle(() => GuncellemeOncesiKontrol(entity));
        }
        catch (IsKuraliException)
        {
            // Reddedilen değişiklik bellekte kalmasın; başka bir SaveChanges onu yazmasın.
            Repository.Reload(entity);
            throw;
        }
        Repository.Update(entity);
        Repository.SaveChanges();
    }

    // --- Kanca (hook) metotlar ---

    // virtual: her entity'nin eklemeye özel kuralı olmayabilir; varsayılanı boştur.
    protected virtual void EklemeOncesiKontrol(T entity) { }

    // virtual: çoğu kural (benzersizlik, format) eklemede ve güncellemede aynıdır;
    // bu yüzden varsayılan olarak ekleme kontrolünü çağırır. Güncellemeye özel kuralı
    // olan servis override edip base.GuncellemeOncesiKontrol(entity)'yi de çağırır.
    protected virtual void GuncellemeOncesiKontrol(T entity) => EklemeOncesiKontrol(entity);

    // abstract: silme her entity'de ilişkili kayıt kontrolü gerektirir; unutulmasın diye zorunlu.
    protected abstract void SilmeOncesiKontrol(T entity);

    /// <summary>
    /// Entity'lerin fırlattığı ArgumentException'ı DogrulamaException'a çevirir
    /// (orijinal hata InnerException olarak korunur).
    /// </summary>
    protected static void DogrulamaIle(Action islem)
    {
        try
        {
            islem();
        }
        catch (ArgumentException ex)
        {
            throw new DogrulamaException(ex);
        }
    }
}
