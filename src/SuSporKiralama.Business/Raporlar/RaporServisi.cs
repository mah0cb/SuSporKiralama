using Microsoft.EntityFrameworkCore;
using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.DataAccess.Repository;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Business.Raporlar;

/// <summary>
/// Rapor sorguları IRepository.Query() üzerinden yazılır; EF Core bunları SQL'e çevirir, böylece
/// gruplama/toplam/sayım veritabanında yapılır ve belleğe yalnızca sonuç satırları gelir.
/// Kullanılan ifadeler (Date, Month, Sum, Count) hem SQL Server'da hem SQLite'ta çevrilebilir;
/// SQLite'ın desteklemediği decimal toplamı EF Core kendi ef_sum fonksiyonuyla yapar.
///
/// GELİR TANIMI: Odemeler tablosundaki tahsilatlar, ödeme tarihine (OdemeTarihi) göre.
/// Kiralamanın tarihi değil, paranın alındığı gün esas alınır. Depozito ve depozitodan
/// yapılan hasar mahsubu Odemeler'de olmadığı için gelire dahil değildir.
/// </summary>
public class RaporServisi(
    IRepository<Odeme> odemeRepository,
    IRepository<Kiralama> kiralamaRepository,
    IRepository<KiralamaDetay> kiralamaDetayRepository,
    IRepository<Ekipman> ekipmanRepository,
    IYetkiServisi yetki,
    TimeProvider zaman,
    IsletmeAyarlari ayarlar) : IRaporServisi
{
    // TPH discriminator sütunu (EkipmanConfiguration): SupBoard / Kano / CanYelegi. Sorguda
    // EF.Property ile okunur, böylece tür adı da veritabanından gelir.
    private const string TurSutunu = "EkipmanTipi";

    private DateTime Simdi => zaman.GetLocalNow().DateTime;

    public List<GunlukGelir> GunlukGelirler(DateOnly baslangic, DateOnly bitis)
    {
        yetki.YetkiKontrol(Islem.RaporGoruntuleme);
        if (bitis < baslangic)
            throw new DogrulamaException("Bitiş tarihi başlangıç tarihinden önce olamaz.");

        var ilk = baslangic.ToDateTime(TimeOnly.MinValue);
        var son = bitis.AddDays(1).ToDateTime(TimeOnly.MinValue); // bitiş günü dahil: [ilk, son)

        // Veritabanında: GROUP BY gün, SUM(Tutar). Yalnızca ödeme olan günler döner.
        var gunler = odemeRepository.Query()
            .Where(o => o.OdemeTarihi >= ilk && o.OdemeTarihi < son)
            .GroupBy(o => o.OdemeTarihi.Date)
            .Select(g => new { Gun = g.Key, Tutar = g.Sum(o => o.Tutar) })
            .ToDictionary(x => DateOnly.FromDateTime(x.Gun), x => x.Tutar);

        // Bellekte: ödeme olmayan günler 0 ile eklenir (grafikte boşluk kalmasın).
        var gunSayisi = bitis.DayNumber - baslangic.DayNumber + 1;
        return Enumerable.Range(0, gunSayisi)
            .Select(i => baslangic.AddDays(i))
            .Select(gun => new GunlukGelir(gun, gunler.GetValueOrDefault(gun)))
            .ToList();
    }

    public List<AylikGelir> AylikGelirler(int yil)
    {
        yetki.YetkiKontrol(Islem.RaporGoruntuleme);
        if (yil is < 2000 or > 9998)
            throw new DogrulamaException("Geçerli bir yıl giriniz.");

        var ilk = new DateTime(yil, 1, 1);
        var son = ilk.AddYears(1);

        var aylar = odemeRepository.Query()
            .Where(o => o.OdemeTarihi >= ilk && o.OdemeTarihi < son)
            .GroupBy(o => o.OdemeTarihi.Month)
            .Select(g => new { Ay = g.Key, Tutar = g.Sum(o => o.Tutar) })
            .ToDictionary(x => x.Ay, x => x.Tutar);

        return Enumerable.Range(1, 12)
            .Select(ay => new AylikGelir(ay, aylar.GetValueOrDefault(ay)))
            .ToList();
    }

    public List<EkipmanKiralamaIstatistigi> EnCokKiralananlar(DateTime baslangic, DateTime bitis, int adet)
    {
        yetki.YetkiKontrol(Islem.RaporGoruntuleme);
        if (bitis <= baslangic)
            throw new DogrulamaException("Bitiş zamanı başlangıç zamanından sonra olmalıdır.");
        if (adet <= 0)
            throw new DogrulamaException("Listelenecek ekipman sayısı sıfırdan büyük olmalıdır.");

        var simdi = Simdi;

        // Veritabanında: filtre ve yalnızca gereken sütunlar (kiralama satırı başına bir satır).
        // Bellekte: süre ve gruplama. Süre bir tarih farkıdır; SQLite tarih farkını (TimeSpan) SQL'e
        // çeviremez, EF.Functions.DateDiffMinute ise SQL Server'a özeldir. Satır sayısı seçilen
        // aralıktaki kiralama sayısı kadar olduğu için bellekte gruplamak yeterlidir.
        var satirlar = kiralamaDetayRepository.Query()
            .Where(d => (d.Kiralama.Durum == KiralamaDurumu.Aktif || d.Kiralama.Durum == KiralamaDurumu.Tamamlandi)
                        && d.Kiralama.BaslangicZamani >= baslangic && d.Kiralama.BaslangicZamani < bitis)
            .Select(d => new
            {
                d.EkipmanId,
                d.Ekipman.Kod,
                Tur = EF.Property<string>(d.Ekipman, TurSutunu),
                d.Kiralama.BaslangicZamani,
                d.Kiralama.GercekBitisZamani,
                d.HesaplananUcret
            })
            .ToList();

        return satirlar
            .GroupBy(s => new { s.EkipmanId, s.Kod, s.Tur })
            .Select(g => new EkipmanKiralamaIstatistigi(
                g.Key.Kod,
                g.Key.Tur,
                g.Count(),
                // Henüz iade edilmemiş kiralamanın süresi şu ana kadar sayılır.
                g.Aggregate(TimeSpan.Zero, (toplam, s) => toplam + ((s.GercekBitisZamani ?? simdi) - s.BaslangicZamani)),
                g.Sum(s => s.HesaplananUcret ?? 0m)))  // aktif kiralamanın ücreti henüz hesaplanmadı
            .OrderByDescending(e => e.KiralanmaSayisi)
            .ThenByDescending(e => e.ToplamSure)
            .ThenBy(e => e.Kod)
            .Take(adet)
            .ToList();
    }

    public List<DolulukOrani> DolulukOranlari(DateOnly baslangic, DateOnly bitis)
    {
        yetki.YetkiKontrol(Islem.RaporGoruntuleme);
        if (bitis < baslangic)
            throw new DogrulamaException("Bitiş tarihi başlangıç tarihinden önce olamaz.");

        var simdi = Simdi;
        var ilk = baslangic.ToDateTime(ayarlar.AcilisSaati);
        var son = bitis.ToDateTime(ayarlar.KapanisSaati);

        // Kullanılabilir ekipman: hizmet dışı olmayan. Bakımdakiler dahildir; hasarlı iade edilen
        // ekipman bakıma girer ve o güne kadarki kiralama saatleri paydada karşılıksız kalmamalı.
        // ponytail: ekipman sayısı bugünkü duruma göre; geçmiş durum tutulmadığı için aralıkta
        // sonradan eklenen/hizmet dışı kalan ekipman hesaba katılamaz. Gerekirse durum geçmişi tablosu.
        // Veritabanında: GROUP BY EkipmanTipi, COUNT(*).
        var ekipmanSayilari = ekipmanRepository.Query()
            .Where(e => e.Durum != EkipmanDurumu.HizmetDisi)
            .GroupBy(e => EF.Property<string>(e, TurSutunu))
            .Select(g => new { Tur = g.Key, Sayi = g.Count() })
            .ToList();

        // Veritabanında: aralıkla çakışan teslim edilmiş kiralama satırları. Bellekte: her günün
        // çalışma saatleriyle kesişim (tarih aritmetiği SQLite'ta SQL'e çevrilemez).
        var kiralamalar = kiralamaDetayRepository.Query()
            .Where(d => d.Ekipman.Durum != EkipmanDurumu.HizmetDisi
                        && (d.Kiralama.Durum == KiralamaDurumu.Aktif || d.Kiralama.Durum == KiralamaDurumu.Tamamlandi)
                        && d.Kiralama.BaslangicZamani < son
                        && (d.Kiralama.GercekBitisZamani == null || d.Kiralama.GercekBitisZamani > ilk))
            .Select(d => new
            {
                Tur = EF.Property<string>(d.Ekipman, TurSutunu),
                d.Kiralama.BaslangicZamani,
                d.Kiralama.GercekBitisZamani
            })
            .ToList();

        var gunSayisi = bitis.DayNumber - baslangic.DayNumber + 1;
        var acikSaat = gunSayisi * ayarlar.GunlukAcikSure.TotalHours;

        return ekipmanSayilari
            .OrderBy(e => e.Tur)
            .Select(e =>
            {
                var kiralananSaat = kiralamalar
                    .Where(k => k.Tur == e.Tur)
                    .Sum(k => CalismaSaatiIcindekiSure(k.BaslangicZamani, k.GercekBitisZamani ?? simdi, baslangic, bitis).TotalHours);
                var oran = e.Sayi == 0 || acikSaat == 0 ? 0 : kiralananSaat / (e.Sayi * acikSaat);
                return new DolulukOrani(e.Tur, e.Sayi, kiralananSaat, acikSaat, oran);
            })
            .ToList();
    }

    /// <summary>
    /// [kiralamaBaslangic, kiralamaBitis] aralığının, ilkGun–sonGun arasındaki her günün çalışma
    /// saatleriyle ([açılış, kapanış]) kesişen toplam süresi. Gece dışarıda kalan saatler sayılmaz.
    /// </summary>
    private TimeSpan CalismaSaatiIcindekiSure(DateTime kiralamaBaslangic, DateTime kiralamaBitis, DateOnly ilkGun, DateOnly sonGun)
    {
        var toplam = TimeSpan.Zero;
        for (var gun = ilkGun; gun <= sonGun; gun = gun.AddDays(1))
        {
            var acilis = gun.ToDateTime(ayarlar.AcilisSaati);
            var kapanis = gun.ToDateTime(ayarlar.KapanisSaati);

            var kesisimBaslangic = kiralamaBaslangic > acilis ? kiralamaBaslangic : acilis;
            var kesisimBitis = kiralamaBitis < kapanis ? kiralamaBitis : kapanis;
            if (kesisimBitis > kesisimBaslangic)
                toplam += kesisimBitis - kesisimBaslangic;
        }
        return toplam;
    }

    public DashboardOzeti DashboardOzeti()
    {
        yetki.YetkiKontrol(Islem.RaporGoruntuleme);

        var simdi = Simdi;
        var bugun = simdi.Date;
        var yarin = bugun.AddDays(1);
        var kiralamalar = kiralamaRepository.Query();
        var ekipmanlar = ekipmanRepository.Query();

        // Her biri ayrı bir SELECT SUM/COUNT sorgusu; satırlar belleğe hiç gelmez.
        return new DashboardOzeti(
            BugunkuGelir: odemeRepository.Query()
                .Where(o => o.OdemeTarihi >= bugun && o.OdemeTarihi < yarin)
                .Sum(o => o.Tutar),
            AktifKiralamaSayisi: kiralamalar.Count(k => k.Durum == KiralamaDurumu.Aktif),
            GecikmisKiralamaSayisi: kiralamalar.Count(k => k.Durum == KiralamaDurumu.Aktif && k.PlanlananBitisZamani < simdi),
            BugunkuRezervasyonSayisi: kiralamalar.Count(k => k.Durum == KiralamaDurumu.Rezerve
                                                             && k.BaslangicZamani >= bugun && k.BaslangicZamani < yarin),
            BakimdakiEkipmanSayisi: ekipmanlar.Count(e => e.Durum == EkipmanDurumu.Bakimda),
            MusaitEkipmanSayisi: ekipmanlar.Count(e => e.Durum == EkipmanDurumu.Musait));
    }
}
