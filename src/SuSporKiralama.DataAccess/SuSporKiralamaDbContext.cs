using Microsoft.EntityFrameworkCore;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.DataAccess;

/// <summary>
/// Uygulamanın veritabanı oturumu. Ayarlar (bağlantı dizesi vb.) dışarıdan
/// DbContextOptions ile verilir; böylece ileride UI katmanında DI ile oluşturulabilir.
/// </summary>
public class SuSporKiralamaDbContext(DbContextOptions<SuSporKiralamaDbContext> options) : DbContext(options)
{
    public DbSet<Personel> Personeller => Set<Personel>();
    public DbSet<Musteri> Musteriler => Set<Musteri>();

    // SupBoard, Kano ve CanYelegi bu küme üzerinden sorgulanır (TPH, tek tablo).
    public DbSet<Ekipman> Ekipmanlar => Set<Ekipman>();
    public DbSet<Kiralama> Kiralamalar => Set<Kiralama>();
    public DbSet<KiralamaDetay> KiralamaDetaylari => Set<KiralamaDetay>();
    public DbSet<HasarKaydi> HasarKayitlari => Set<HasarKaydi>();
    public DbSet<Odeme> Odemeler => Set<Odeme>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Konfigurasyonlar klasöründeki tüm IEntityTypeConfiguration sınıflarını uygular.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SuSporKiralamaDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Tüm modele uygulanan ortak kurallar; her entity'de tek tek yazmamak için burada.
        // Parasal alanlar: 10 basamak, 2'si ondalık (ör. 99999999.99).
        configurationBuilder.Properties<decimal>().HavePrecision(10, 2);

        // Enum'lar sayı yerine metin olarak saklanır; veritabanında "Kirada" okumak 1'den anlaşılırdır.
        configurationBuilder.Properties<Enum>().HaveConversion<string>().HaveMaxLength(20);
    }
}
