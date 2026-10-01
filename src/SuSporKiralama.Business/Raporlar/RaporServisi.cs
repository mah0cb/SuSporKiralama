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
    IRepository<Ekipman> ekipmanRepository,
    IYetkiServisi yetki,
    TimeProvider zaman) : IRaporServisi
{
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
