using Microsoft.EntityFrameworkCore;

namespace SuSporKiralama.Tests;

/// <summary>Test veritabanının beklendiği gibi kurulduğunu doğrular.</summary>
public class AltyapiTestleri : IDisposable
{
    private readonly TestVeritabani _db = new();

    public void Dispose() => _db.Dispose();

    [Fact]
    public void Veritabani_OrnekVeriOlmadanBosOlusturulur()
    {
        Assert.Empty(_db.Context.Personeller);
        Assert.Empty(_db.Context.Musteriler);
        Assert.Empty(_db.Context.Ekipmanlar);
    }

    [Fact]
    public void Veritabani_UniqueIndexUygulanir()
    {
        _db.MusteriEkle("05321112233");

        Assert.Throws<DbUpdateException>(() => _db.MusteriEkle("05321112233"));
    }
}
