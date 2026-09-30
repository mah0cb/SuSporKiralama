using SuSporKiralama.DataAccess.Guvenlik;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.DataAccess.Seed;

/// <summary>
/// Geliştirme ve sunum için örnek veri. DbContext'teki UseSeeding üzerinden
/// "dotnet ef database update" veya Database.Migrate() sırasında çalışır.
/// İdempotenttir: veritabanında personel varsa hiçbir şey eklemez.
/// </summary>
public static class OrnekVeri
{
    public static void Yukle(SuSporKiralamaDbContext context)
    {
        if (context.Personeller.Any())
            return;

        // Tüm ekleme tek transaction'da: yarıda hata olursa hiçbir kayıt kalmaz,
        // böylece bir sonraki denemede yukarıdaki kontrol yanlışlıkla atlamaz.
        // (Migrate zaten bir transaction açtıysa onu kullanırız.)
        using var transaction = context.Database.CurrentTransaction is null
            ? context.Database.BeginTransaction()
            : null;

        // 1) Ana kayıtlar: personel, müşteri, ekipman
        var admin = new Personel("Sistem Yöneticisi", "admin", SifreHasher.Hashle("Admin123!"), Rol.Admin);
        var personel = new Personel("Deniz Kaya", "personel", SifreHasher.Hashle("Personel123!"), Rol.Personel);
        context.Personeller.AddRange(admin, personel);

        var musteriler = new List<Musteri>
        {
            new("Ayşe", "Yılmaz", "05321112233", DeneyimSeviyesi.Baslangic) { Eposta = "ayse.yilmaz@example.com" },
            new("Mehmet", "Demir", "05332223344", DeneyimSeviyesi.Orta),
            new("Elif", "Şahin", "05343334455", DeneyimSeviyesi.Ileri) { Eposta = "elif.sahin@example.com", Notlar = "Kano turlarını tercih ediyor." },
            new("Can", "Öztürk", "05354445566", DeneyimSeviyesi.Baslangic),
            new("Zeynep", "Arslan", "05365556677", DeneyimSeviyesi.Orta) { Eposta = "zeynep.arslan@example.com" },
            new("Burak", "Koç", "05376667788", DeneyimSeviyesi.Ileri),
        };
        context.Musteriler.AddRange(musteriler);

        var suplar = new List<SupBoard>
        {
            new("SUP-001", "Aqua Marina", "Beast", 150m, 1500m, 320, 120, SupBoardTipi.Sisme),
            new("SUP-002", "Aqua Marina", "Beast", 150m, 1500m, 320, 120, SupBoardTipi.Sisme),
            new("SUP-003", "Aqua Marina", "Magma", 175m, 1750m, 340, 140, SupBoardTipi.Sisme),
            new("SUP-004", "Red Paddle", "Ride 10'6", 200m, 2000m, 320, 110, SupBoardTipi.Sisme),
            new("SUP-005", "Red Paddle", "Voyager 12'6", 225m, 2500m, 381, 150, SupBoardTipi.Sisme),
            new("SUP-006", "Starboard", "Go 11'2", 250m, 3000m, 340, 130, SupBoardTipi.Sert),
            new("SUP-007", "Starboard", "Touring 12'6", 250m, 3000m, 381, 140, SupBoardTipi.Sert),
            new("SUP-008", "Starboard", "Go 10'8", 225m, 2500m, 325, 120, SupBoardTipi.Sert) { Durum = EkipmanDurumu.Bakimda },
        };
        var kanolar = new List<Kano>
        {
            new("KANO-001", "Pelican", "Argo 100X", 600m, 2500m, 1),
            new("KANO-002", "Old Town", "Discovery 169", 800m, 4000m, 2),
            new("KANO-003", "Old Town", "Guide 160", 900m, 4500m, 3),
        };
        var bedenler = new[] { Beden.S, Beden.S, Beden.M, Beden.M, Beden.M, Beden.L, Beden.L, Beden.L, Beden.XL, Beden.XL };
        var yelekler = bedenler
            .Select((beden, i) => new CanYelegi($"YLK-{i + 1:000}", "Decathlon", "Itiwit 100N", 50m, 200m, beden))
            .ToList();

        context.Ekipmanlar.AddRange(suplar);
        context.Ekipmanlar.AddRange(kanolar);
        context.Ekipmanlar.AddRange(yelekler);

        context.SaveChanges(); // Id'ler oluştu; kiralamalarda kullanılabilir.

        // 2) Kiralamalar: 3 tamamlanmış + 1 aktif + 1 rezervasyon.
        // Durumlar entity metotlarıyla (TeslimEt/Tamamla) değiştirilir; ücret, depozito ve
        // ekipman durumu uygulamadaki iade akışıyla aynı kurallarla hesaplanır.
        var bugun = DateTime.Today;
        var simdi = DateTime.Now;

        var k1 = TeslimEdilmis(musteriler[0], personel, bugun.AddDays(-10).AddHours(10), TimeSpan.FromHours(2), suplar[0], yelekler[2]);
        k1.Tamamla(k1.BaslangicZamani.AddHours(2).AddMinutes(20)); // 20 dk geç: 3 saat ücretlenir

        var k2 = TeslimEdilmis(musteriler[2], admin, bugun.AddDays(-7).AddHours(9), TimeSpan.FromHours(4), kanolar[1], yelekler[3], yelekler[5]);
        k2.Tamamla(k2.BaslangicZamani.AddHours(4));

        // k3 hasarlı iade: 300 TL hasar depozitodan (1750 TL) düşülür, ekipman Bakımda'ya alınır.
        var k3 = TeslimEdilmis(musteriler[1], personel, bugun.AddDays(-3).AddHours(14), TimeSpan.FromHours(2), suplar[2]);
        var k3Bitis = k3.BaslangicZamani.AddMinutes(90);
        k3.Detaylar.First().HasarKayitlari.Add(new HasarKaydi(0, "Kanat (fin) yuvasında çatlak.", 300m) { KayitTarihi = k3Bitis });
        k3.Tamamla(k3Bitis);

        var aktif = TeslimEdilmis(musteriler[3], personel, simdi.AddHours(-1), TimeSpan.FromHours(3), suplar[4], yelekler[6]);

        var yarin = bugun.AddDays(1).AddHours(10);
        var rezervasyon = KiralamaOlustur(musteriler[4], personel, yarin, TimeSpan.FromHours(2), suplar[5], yelekler[7]);
        rezervasyon.OlusturmaTarihi = simdi;

        context.Kiralamalar.AddRange(k1, k2, k3, aktif, rezervasyon);
        context.SaveChanges();

        // 3) Ödemeler (kiralama Id'leri artık belli). Ödeme yalnızca tamamlanmış kiralamaya alınır.
        context.Odemeler.AddRange(
            new Odeme(k1.Id, k1.KalanBorc, OdemeTipi.Nakit, k1.GercekBitisZamani!.Value),
            // Bir kiralamanın birden fazla ödemesi olabilir (1-N):
            new Odeme(k2.Id, 500m, OdemeTipi.Nakit, k2.GercekBitisZamani!.Value) { Aciklama = "Nakit kısmı" },
            new Odeme(k2.Id, k2.KalanBorc - 500m, OdemeTipi.KrediKarti, k2.GercekBitisZamani!.Value) { Aciklama = "Kalan tutar" },
            new Odeme(k3.Id, k3.KalanBorc, OdemeTipi.Havale, k3.GercekBitisZamani!.Value) { Aciklama = "Hasar bedeli depozitodan düşüldü" });

        context.SaveChanges();
        transaction?.Commit();
    }

    private static Kiralama KiralamaOlustur(Musteri musteri, Personel personel, DateTime baslangic,
        TimeSpan planlananSure, params Ekipman[] ekipmanlar)
    {
        var kiralama = new Kiralama(musteri.Id, personel.Id, baslangic, baslangic + planlananSure)
        {
            OlusturmaTarihi = baslangic
        };
        foreach (var ekipman in ekipmanlar)
            kiralama.Detaylar.Add(new KiralamaDetay(ekipman)); // fiyat kopyalanır
        return kiralama;
    }

    // Başlangıç anında teslim edilmiş (Aktif) kiralama: depozito alınır, ekipmanlar Kirada olur.
    private static Kiralama TeslimEdilmis(Musteri musteri, Personel personel, DateTime baslangic,
        TimeSpan planlananSure, params Ekipman[] ekipmanlar)
    {
        var kiralama = KiralamaOlustur(musteri, personel, baslangic, planlananSure, ekipmanlar);
        kiralama.TeslimEt(baslangic);
        return kiralama;
    }
}
