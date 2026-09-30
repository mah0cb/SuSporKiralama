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

        // 2) Kiralamalar: 3 tamamlanmış + 1 aktif
        var bugun = DateTime.Today;

        var k1 = KiralamaOlustur(musteriler[0], personel, bugun.AddDays(-10).AddHours(10), TimeSpan.FromHours(2), suplar[0], yelekler[2]);
        var k2 = KiralamaOlustur(musteriler[2], admin, bugun.AddDays(-7).AddHours(9), TimeSpan.FromHours(4), kanolar[1], yelekler[3], yelekler[5]);
        var k3 = KiralamaOlustur(musteriler[1], personel, bugun.AddDays(-3).AddHours(14), TimeSpan.FromHours(2), suplar[2]);
        Tamamla(k1, k1.BaslangicZamani.AddHours(2).AddMinutes(20)); // 20 dk geç: 3 saat ücretlenir
        Tamamla(k2, k2.BaslangicZamani.AddHours(4));
        Tamamla(k3, k3.BaslangicZamani.AddMinutes(90));

        var simdi = DateTime.Now;
        var aktif = KiralamaOlustur(musteriler[3], personel, simdi.AddHours(-1), TimeSpan.FromHours(3), suplar[4], yelekler[6]);
        foreach (var detay in aktif.Detaylar)
            detay.Ekipman.Durum = EkipmanDurumu.Kirada;

        context.Kiralamalar.AddRange(k1, k2, k3, aktif);
        context.SaveChanges();

        // 3) Hasar ve ödemeler (kiralama/detay Id'leri artık belli)
        var hasarliDetay = k3.Detaylar.First();
        var hasar = new HasarKaydi(hasarliDetay.Id, "Kanat (fin) yuvasında çatlak.", 300m) { KayitTarihi = k3.GercekBitisZamani!.Value };
        context.HasarKayitlari.Add(hasar);
        k3.ToplamUcret += hasar.HasarBedeli;

        context.Odemeler.AddRange(
            new Odeme(k1.Id, k1.ToplamUcret!.Value, OdemeTipi.Nakit, k1.GercekBitisZamani!.Value),
            // Bir kiralamanın birden fazla ödemesi olabilir (1-N):
            new Odeme(k2.Id, 500m, OdemeTipi.Nakit, k2.BaslangicZamani) { Aciklama = "Ön ödeme" },
            new Odeme(k2.Id, k2.ToplamUcret!.Value - 500m, OdemeTipi.KrediKarti, k2.GercekBitisZamani!.Value) { Aciklama = "Kalan tutar" },
            new Odeme(k3.Id, k3.ToplamUcret!.Value, OdemeTipi.Havale, k3.GercekBitisZamani!.Value) { Aciklama = "Hasar bedeli dahil" },
            new Odeme(aktif.Id, 200m, OdemeTipi.KrediKarti, aktif.BaslangicZamani) { Aciklama = "Ön ödeme" });

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

    // Geçmiş kiralamaları tamamlanmış hale getirir; ücretler polimorfik UcretHesapla ile hesaplanır.
    private static void Tamamla(Kiralama kiralama, DateTime bitis)
    {
        var sure = bitis - kiralama.BaslangicZamani;
        foreach (var detay in kiralama.Detaylar)
            detay.HesaplananUcret = detay.Ekipman.UcretHesapla(sure, detay.UygulananBirimUcret);

        kiralama.GercekBitisZamani = bitis;
        kiralama.Durum = KiralamaDurumu.Tamamlandi;
        kiralama.ToplamUcret = kiralama.Detaylar.Sum(d => d.HesaplananUcret);
    }
}
