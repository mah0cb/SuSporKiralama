using Microsoft.Extensions.Time.Testing;
using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Business.Raporlar;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Tests;

public class RaporServisiTestleri : IDisposable
{
    // "Şimdi" 15 Temmuz 2026 10:00.
    private static readonly DateTime Bugun = new(2026, 7, 15);
    private static DateTime Gun(int gun, int saat = 0, int dakika = 0) => new DateTime(2026, 7, gun, saat, dakika, 0);

    private readonly TestVeritabani _db = new();
    private readonly FakeTimeProvider _zaman = new(new DateTimeOffset(Bugun.AddHours(10), TimeSpan.Zero));
    private readonly Musteri _musteri;
    private readonly Personel _personel;

    public RaporServisiTestleri()
    {
        _musteri = _db.MusteriEkle();
        _personel = _db.PersonelEkle();
    }

    public void Dispose() => _db.Dispose();

    private RaporServisi Servis(Oturum? oturum = null) =>
        new(_db.Repo<Odeme>(), _db.Repo<Kiralama>(), _db.Repo<KiralamaDetay>(), _db.Repo<Ekipman>(),
            (oturum ?? TestOturumu.AdminOlarakGiris()).Yetki(), _zaman, IsletmeAyarlari.Varsayilan); // 09:00–19:00

    private Kiralama Tamamlanmis(Ekipman ekipman) =>
        _db.KiralamaKur(_musteri, _personel, Gun(1, 10), Gun(1, 12), KiralamaDurumu.Tamamlandi, ekipman);

    private void OdemeEkle(Kiralama kiralama, DateTime tarih, decimal tutar)
    {
        _db.Context.Odemeler.Add(new Odeme(kiralama.Id, tutar, OdemeTipi.Nakit, tarih));
        _db.Context.SaveChanges();
    }

    /// <summary>13 Tem: 100 + 50, 15 Tem (bugün): 200, 20 Haz: 70, geçen yıl son dakika: 999.</summary>
    private void OdemeleriKur()
    {
        var kiralama = Tamamlanmis(_db.SupEkle());
        OdemeEkle(kiralama, Gun(13, 11), 100m);
        OdemeEkle(kiralama, Gun(13, 15, 30), 50m);
        OdemeEkle(kiralama, Gun(15, 9), 200m);
        OdemeEkle(kiralama, new DateTime(2026, 6, 20, 12, 0, 0), 70m);
        OdemeEkle(kiralama, new DateTime(2025, 12, 31, 23, 59, 0), 999m);
    }

    // --- Günlük gelir ---

    [Fact]
    public void GunlukGelir_OdemeOlmayanGunler0()
    {
        OdemeleriKur();

        var sonuc = Servis().GunlukGelirler(new DateOnly(2026, 7, 12), new DateOnly(2026, 7, 15));

        Assert.Equal(
            [
                new GunlukGelir(new DateOnly(2026, 7, 12), 0m),
                new GunlukGelir(new DateOnly(2026, 7, 13), 150m),
                new GunlukGelir(new DateOnly(2026, 7, 14), 0m),
                new GunlukGelir(new DateOnly(2026, 7, 15), 200m)
            ], sonuc);
    }

    [Fact]
    public void GunlukGelir_TekGun_SinirlarDahil()
    {
        OdemeleriKur();

        var sonuc = Servis().GunlukGelirler(new DateOnly(2026, 7, 13), new DateOnly(2026, 7, 13));

        Assert.Equal(150m, Assert.Single(sonuc).Tutar);
    }

    [Fact]
    public void GunlukGelir_BosVeritabani_HepsiSifir()
    {
        var sonuc = Servis().GunlukGelirler(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 31));

        Assert.Equal(31, sonuc.Count);
        Assert.All(sonuc, g => Assert.Equal(0m, g.Tutar));
    }

    [Fact]
    public void GunlukGelir_TersAralik_DogrulamaException()
    {
        Assert.Throws<DogrulamaException>(() =>
            Servis().GunlukGelirler(new DateOnly(2026, 7, 15), new DateOnly(2026, 7, 14)));
    }

    // --- Aylık gelir ---

    [Fact]
    public void AylikGelir_OnIkiAyBoslar0BaskaYilHaric()
    {
        OdemeleriKur();

        var sonuc = Servis().AylikGelirler(2026);

        Assert.Equal(Enumerable.Range(1, 12), sonuc.Select(a => a.Ay));
        Assert.Equal(70m, sonuc[5].Tutar);   // Haziran
        Assert.Equal(350m, sonuc[6].Tutar);  // Temmuz: 100 + 50 + 200
        Assert.Equal(420m, sonuc.Sum(a => a.Tutar)); // 2025'teki 999 dahil değil
    }

    // --- En çok kiralanan ekipmanlar ---

    private void KiralamalariKur()
    {
        var sup = _db.SupEkle();            // 150 TL/saat
        var kano = _db.KanoEkle();          // 600 TL / 4 saatlik blok
        var yelek = _db.YelekEkle();        // 50 TL sabit
        var sup2 = _db.SupEkle("SUP-002");

        _db.KiralamaKur(_musteri, _personel, Gun(1, 10), Gun(1, 12), KiralamaDurumu.Tamamlandi, sup);  // 2 saat, 300
        _db.KiralamaKur(_musteri, _personel, Gun(2, 10), Gun(2, 11), KiralamaDurumu.Tamamlandi, sup);  // 1 saat, 150
        _db.KiralamaKur(_musteri, _personel, Gun(3, 10), Gun(3, 14), KiralamaDurumu.Tamamlandi, kano); // 4 saat, 600
        _db.KiralamaKur(_musteri, _personel, Gun(15, 9), Gun(15, 12), KiralamaDurumu.Aktif, yelek);    // şimdiye kadar 1 saat
        _db.KiralamaKur(_musteri, _personel, Gun(16, 10), Gun(16, 12), KiralamaDurumu.Rezerve, sup2);  // sayılmaz
        _db.KiralamaKur(_musteri, _personel, Gun(4, 10), Gun(4, 12), KiralamaDurumu.IptalEdildi, sup2); // sayılmaz
        _db.KiralamaKur(_musteri, _personel, new DateTime(2026, 6, 30, 10, 0, 0), new DateTime(2026, 6, 30, 18, 0, 0),
            KiralamaDurumu.Tamamlandi, yelek);                                                           // aralık dışı
    }

    [Fact]
    public void EnCokKiralananlar_SiralamaSureVeUcret()
    {
        KiralamalariKur();

        var sonuc = Servis().EnCokKiralananlar(Gun(1), Gun(16), 10);

        Assert.Equal(
            [
                new EkipmanKiralamaIstatistigi("SUP-001", "SupBoard", 2, TimeSpan.FromHours(3), 450m),
                new EkipmanKiralamaIstatistigi("KANO-001", "Kano", 1, TimeSpan.FromHours(4), 600m),
                new EkipmanKiralamaIstatistigi("YLK-001", "CanYelegi", 1, TimeSpan.FromHours(1), 0m) // aktif: ücret henüz yok
            ], sonuc);
    }

    [Fact]
    public void EnCokKiralananlar_IlkN()
    {
        KiralamalariKur();

        var sonuc = Servis().EnCokKiralananlar(Gun(1), Gun(16), 1);

        Assert.Equal("SUP-001", Assert.Single(sonuc).Kod);
    }

    [Fact]
    public void EnCokKiralananlar_BosAralik_BosListe()
    {
        KiralamalariKur();

        Assert.Empty(Servis().EnCokKiralananlar(Gun(20), Gun(25), 5));
    }

    [Fact]
    public void EnCokKiralananlar_GecersizParametre_DogrulamaException()
    {
        Assert.Throws<DogrulamaException>(() => Servis().EnCokKiralananlar(Gun(2), Gun(1), 5));
        Assert.Throws<DogrulamaException>(() => Servis().EnCokKiralananlar(Gun(1), Gun(2), 0));
    }

    // --- Doluluk oranı ---

    [Fact]
    public void Doluluk_TurBazindaCalismaSaatleriIcinde()
    {
        var sup1 = _db.SupEkle("SUP-001");
        var sup2 = _db.SupEkle("SUP-002");
        var hizmetDisi = _db.SupEkle("SUP-003");
        _db.KanoEkle();

        _db.KiralamaKur(_musteri, _personel, Gun(14, 9), Gun(14, 12), KiralamaDurumu.Tamamlandi, sup1);   // 3 saat
        _db.KiralamaKur(_musteri, _personel, Gun(14, 17), Gun(15, 8), KiralamaDurumu.Tamamlandi, sup2);   // gece taşar: 17–19 = 2 saat
        _db.KiralamaKur(_musteri, _personel, Gun(14, 13), Gun(14, 15), KiralamaDurumu.IptalEdildi, sup1); // sayılmaz
        _db.KiralamaKur(_musteri, _personel, Gun(14, 9), Gun(14, 19), KiralamaDurumu.Tamamlandi, hizmetDisi);
        hizmetDisi.Durum = EkipmanDurumu.HizmetDisi;                                                       // hiç sayılmaz
        _db.Context.SaveChanges();

        var gun = new DateOnly(2026, 7, 14);
        var sonuc = Servis().DolulukOranlari(gun, gun);

        Assert.Equal(
            [
                new DolulukOrani("Kano", 1, 0, 10, 0),
                new DolulukOrani("SupBoard", 2, 5, 10, 0.25) // 5 / (2 × 10)
            ], sonuc);
    }

    [Fact]
    public void Doluluk_AktifKiralamaSimdiyeKadarSayilir()
    {
        var sup = _db.SupEkle();
        _db.KiralamaKur(_musteri, _personel, Gun(15, 8), Gun(15, 12), KiralamaDurumu.Aktif, sup); // 09–10 arası sayılır

        var bugun = DateOnly.FromDateTime(Bugun);
        var doluluk = Assert.Single(Servis().DolulukOranlari(bugun, bugun));

        Assert.Equal(1, doluluk.KiralananSaat);
        Assert.Equal(0.1, doluluk.Oran, 10);
    }

    [Fact]
    public void Doluluk_BirkacGun_AcikSaatGunSayisiKadar()
    {
        var sup = _db.SupEkle();
        _db.KiralamaKur(_musteri, _personel, Gun(13, 9), Gun(14, 19), KiralamaDurumu.Tamamlandi, sup); // iki tam gün

        var doluluk = Assert.Single(Servis().DolulukOranlari(new DateOnly(2026, 7, 13), new DateOnly(2026, 7, 16)));

        Assert.Equal(40, doluluk.AcikSaat);
        Assert.Equal(20, doluluk.KiralananSaat);
        Assert.Equal(0.5, doluluk.Oran, 10);
    }

    [Fact]
    public void Doluluk_TersAralik_DogrulamaException()
    {
        Assert.Throws<DogrulamaException>(() =>
            Servis().DolulukOranlari(new DateOnly(2026, 7, 15), new DateOnly(2026, 7, 14)));
    }

    // --- Dashboard ---

    [Fact]
    public void Dashboard_TumAlanlar()
    {
        OdemeleriKur();                                                       // bugünkü gelir 200, SUP-001 müsait
        _db.KiralamaKur(_musteri, _personel, Gun(15, 9), Gun(15, 12), KiralamaDurumu.Aktif, _db.SupEkle("SUP-002"));
        _db.KiralamaKur(_musteri, _personel, Gun(15, 6), Gun(15, 9), KiralamaDurumu.Aktif, _db.KanoEkle()); // gecikmiş
        var yelek = _db.YelekEkle();                                          // müsait
        _db.KiralamaKur(_musteri, _personel, Gun(15, 14), Gun(15, 16), KiralamaDurumu.Rezerve, yelek);
        _db.KiralamaKur(_musteri, _personel, Gun(16, 14), Gun(16, 16), KiralamaDurumu.Rezerve, yelek);     // yarın
        _db.KiralamaKur(_musteri, _personel, Gun(15, 17), Gun(15, 18), KiralamaDurumu.IptalEdildi, yelek);
        _db.SupEkle("SUP-003", EkipmanDurumu.Bakimda);

        var ozet = Servis().DashboardOzeti();

        Assert.Equal(new DashboardOzeti(
            BugunkuGelir: 200m,
            AktifKiralamaSayisi: 2,
            GecikmisKiralamaSayisi: 1,
            BugunkuRezervasyonSayisi: 1,
            BakimdakiEkipmanSayisi: 1,
            MusaitEkipmanSayisi: 2), ozet);
    }

    [Fact]
    public void Dashboard_BosVeritabani_HepsiSifir()
    {
        Assert.Equal(new DashboardOzeti(0m, 0, 0, 0, 0, 0), Servis().DashboardOzeti());
    }

    // --- Yetki ---

    [Fact]
    public void PersonelRolu_RaporGoremez()
    {
        var servis = Servis(TestOturumu.PersonelOlarakGiris());

        Assert.Throws<YetkisizIslemException>(() => servis.DashboardOzeti());
        Assert.Throws<YetkisizIslemException>(() => servis.AylikGelirler(2026));
        Assert.Throws<YetkisizIslemException>(() => servis.GunlukGelirler(DateOnly.FromDateTime(Bugun), DateOnly.FromDateTime(Bugun)));
        Assert.Throws<YetkisizIslemException>(() => servis.EnCokKiralananlar(Gun(1), Gun(16), 5));
        Assert.Throws<YetkisizIslemException>(() => servis.DolulukOranlari(DateOnly.FromDateTime(Bugun), DateOnly.FromDateTime(Bugun)));
    }

    [Fact]
    public void OturumYok_RaporGoremez()
    {
        Assert.Throws<YetkisizIslemException>(() => Servis(new Oturum()).DashboardOzeti());
    }
}
