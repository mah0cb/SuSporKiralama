using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SuSporKiralama.DataAccess;

/// <summary>
/// Sadece "dotnet ef" komutları (migration, database update) için kullanılır.
/// Henüz UI projesi olmadığından EF araçları DbContext'i bu sınıf üzerinden oluşturur.
/// Bağlantı dizesi koda gömülmez; proje klasöründeki appsettings.json'dan okunur.
/// </summary>
public class SuSporKiralamaDbContextFactory : IDesignTimeDbContextFactory<SuSporKiralamaDbContext>
{
    public SuSporKiralamaDbContext CreateDbContext(string[] args)
    {
        // dotnet ef, çalışma klasörü olarak DataAccess proje klasörünü kullanır.
        var klasor = Directory.GetCurrentDirectory();
        if (!File.Exists(Path.Combine(klasor, "appsettings.json")))
            throw new FileNotFoundException(
                $"appsettings.json bulunamadı ({klasor}). appsettings.example.json dosyasını appsettings.json adıyla kopyalayın.");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(klasor)
            .AddJsonFile("appsettings.json")
            .Build();

        var baglantiDizesi = configuration.GetConnectionString("SuSporKiralama")
            ?? throw new InvalidOperationException("appsettings.json içinde 'ConnectionStrings:SuSporKiralama' tanımlı değil.");

        var options = new DbContextOptionsBuilder<SuSporKiralamaDbContext>()
            .UseSqlServer(baglantiDizesi)
            .Options;

        return new SuSporKiralamaDbContext(options);
    }
}
