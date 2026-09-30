using Microsoft.EntityFrameworkCore;
using SuSporKiralama.DataAccess.Seed;
using SuSporKiralama.Entities;

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

    [Fact]
    public void OrnekVeri_TutarliVeIdempotent()
    {
        OrnekVeri.Yukle(_db.Context);
        OrnekVeri.Yukle(_db.Context); // ikinci çağrı hiçbir şey eklememeli

        var kiralamalar = _db.Context.Kiralamalar
            .Include(k => k.Odemeler)
            .Include(k => k.Detaylar).ThenInclude(d => d.HasarKayitlari)
            .ToList();
        Assert.Equal(5, kiralamalar.Count);

        var tamamlananlar = kiralamalar.Where(k => k.Durum == KiralamaDurumu.Tamamlandi).ToList();
        Assert.Equal(3, tamamlananlar.Count);
        Assert.All(tamamlananlar, k => Assert.Equal(0m, k.KalanBorc));

        var hasarli = Assert.Single(tamamlananlar, k => k.HasarBedeliToplami > 0);
        Assert.Equal(DepozitoDurumu.KismenIadeEdildi, hasarli.DepozitoDurumu);
        Assert.Equal(300m, hasarli.DepozitoMahsupTutari);

        Assert.Equal(DepozitoDurumu.Alindi, Assert.Single(kiralamalar, k => k.Durum == KiralamaDurumu.Aktif).DepozitoDurumu);
        Assert.Equal(DepozitoDurumu.Alinmadi, Assert.Single(kiralamalar, k => k.Durum == KiralamaDurumu.Rezerve).DepozitoDurumu);
    }
}
