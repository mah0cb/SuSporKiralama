using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using SuSporKiralama.DataAccess;
using SuSporKiralama.DataAccess.Repository;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Tests;

/// <summary>
/// Her test için ayrı, bellekte çalışan bir SQLite veritabanı. SQLite, SQL Server gibi
/// foreign key ve unique index kısıtlarını gerçekten uygular; LocalDB'ye ihtiyaç kalmaz.
/// Bağlantı kapanınca veritabanı yok olur, bu yüzden nesne yaşadığı sürece açık tutulur.
/// </summary>
public sealed class TestVeritabani : IDisposable
{
    private readonly SqliteConnection _baglanti;

    public SuSporKiralamaDbContext Context { get; }

    public TestVeritabani()
    {
        _baglanti = new SqliteConnection("DataSource=:memory:");
        _baglanti.Open();

        Context = new SuSporKiralamaDbContext(
            new DbContextOptionsBuilder<SuSporKiralamaDbContext>().UseSqlite(_baglanti).Options);

        // EnsureCreated yerine CreateTables: EnsureCreated örnek veriyi (UseSeeding) de yükler,
        // CreateTables yalnızca tabloları oluşturur. Böylece her test kendi verisini kurar.
        // (Migration'lar SQL Server'a özel olduğu için kullanılmaz; şema modelden üretilir.)
        Context.GetService<IRelationalDatabaseCreator>().CreateTables();
    }

    public IRepository<T> Repo<T>() where T : BaseEntity => new EfRepository<T>(Context);

    // --- Servisleri kullanmadan doğrudan veri kuran yardımcılar ---

    public Musteri MusteriEkle(string telefon = "05320000000")
    {
        var musteri = new Musteri("Test", "Müşteri", telefon, DeneyimSeviyesi.Orta);
        Context.Musteriler.Add(musteri);
        Context.SaveChanges();
        return musteri;
    }

    // Şifre bu testlerde önemsiz; gerçek hash üretmek testleri yavaşlatır.
    public Personel PersonelEkle(string kullaniciAdi = "personel", Rol rol = Rol.Personel)
    {
        var personel = new Personel("Test Personel", kullaniciAdi, "test-hash", rol);
        Context.Personeller.Add(personel);
        Context.SaveChanges();
        return personel;
    }

    public SupBoard SupEkle(string kod = "SUP-001", EkipmanDurumu durum = EkipmanDurumu.Musait)
    {
        var sup = new SupBoard(kod, "Aqua Marina", "Beast", 150m, 1500m, 320, 120, SupBoardTipi.Sisme) { Durum = durum };
        Context.Ekipmanlar.Add(sup);
        Context.SaveChanges();
        return sup;
    }

    /// <summary>Servisi kullanmadan geçmiş (Tamamlandi) ya da süren (Aktif) bir kiralama kaydı ekler.</summary>
    public Kiralama KiralamaEkle(Musteri musteri, Personel personel, Ekipman ekipman, bool aktif = false)
    {
        var baslangic = DateTime.Now.AddHours(-3);
        var kiralama = new Kiralama(musteri.Id, personel.Id, baslangic, baslangic.AddHours(2));
        kiralama.Detaylar.Add(new KiralamaDetay(ekipman));
        kiralama.TeslimEt(baslangic);
        if (!aktif)
            kiralama.Tamamla(baslangic.AddHours(2));

        Context.Kiralamalar.Add(kiralama);
        Context.SaveChanges();
        return kiralama;
    }

    public void Dispose()
    {
        Context.Dispose();
        _baglanti.Dispose();
    }
}
