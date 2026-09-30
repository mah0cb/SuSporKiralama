using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.DataAccess.Repository;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Business.Servisler;

/// <summary>
/// Ayrı bir servis: kiralama servisi ve 5. aşamadaki yapay zeka önerisi yalnızca
/// müsaitliğe ihtiyaç duyar, kiralama işlemlerine bağımlı olmaları gerekmez.
/// "Şimdi" TimeProvider'dan alınır; testlerde zaman FakeTimeProvider ile kontrol edilir.
/// </summary>
public class MusaitlikServisi(
    IRepository<Ekipman> ekipmanRepository,
    IRepository<KiralamaDetay> kiralamaDetayRepository,
    TimeProvider zaman) : IMusaitlikServisi
{
    public bool MusaitMi(int ekipmanId, DateTime baslangic, DateTime bitis, int? haricKiralamaId = null)
    {
        AralikKontrol(baslangic, bitis);
        var ekipman = ekipmanRepository.GetById(ekipmanId) ?? throw new KayitBulunamadiException("Ekipman", ekipmanId);

        return KiralanabilirDurumda(ekipman.Durum)
            && !DoluEkipmanIdleri(baslangic, bitis, haricKiralamaId).Contains(ekipmanId);
    }

    public List<TEkipman> MusaitEkipmanlariGetir<TEkipman>(DateTime baslangic, DateTime bitis) where TEkipman : Ekipman
    {
        AralikKontrol(baslangic, bitis);
        var dolu = DoluEkipmanIdleri(baslangic, bitis, haricKiralamaId: null);

        // "e is TEkipman" discriminator sütunu üzerinden sorguya çevrilir (EkipmanServisi.Filtrele gibi).
        return ekipmanRepository
            .Find(e => e is TEkipman && e.Durum != EkipmanDurumu.Bakimda && e.Durum != EkipmanDurumu.HizmetDisi)
            .Where(e => !dolu.Contains(e.Id))
            .Cast<TEkipman>()
            .ToList();
    }

    /// <summary>
    /// [baslangic, bitis) aralığıyla çakışan Rezerve/Aktif kiralamalardaki ekipmanlar.
    /// İki aralık, biri diğeri bitmeden başlıyorsa çakışır: k.Baslangic &lt; bitis ve kEtkinBitis &gt; baslangic.
    /// Gecikmiş Aktif kiralamada ekipman hâlâ müşteridedir; etkin bitiş = max(planlanan bitiş, şimdi).
    /// </summary>
    private HashSet<int> DoluEkipmanIdleri(DateTime baslangic, DateTime bitis, int? haricKiralamaId)
    {
        var simdi = zaman.GetLocalNow().DateTime;
        // İade edilmemiş ekipman şu an hâlâ müşteridedir, yani etkin bitişi şimdiden sonradır:
        // şimdi veya daha önce başlayan her istek bu kiralamayla çakışır.
        var aktifKiralamaHalaDolu = baslangic <= simdi;
        var haricId = haricKiralamaId ?? 0;            // Id 0 veritabanında olmaz

        return kiralamaDetayRepository
            .Find(d => (d.Kiralama.Durum == KiralamaDurumu.Rezerve || d.Kiralama.Durum == KiralamaDurumu.Aktif)
                       && d.KiralamaId != haricId
                       && d.Kiralama.BaslangicZamani < bitis
                       && (d.Kiralama.PlanlananBitisZamani > baslangic
                           || (aktifKiralamaHalaDolu && d.Kiralama.Durum == KiralamaDurumu.Aktif)))
            .Select(d => d.EkipmanId)
            .ToHashSet();
    }

    // Bakımdaki ve hizmet dışı ekipman hiçbir aralıkta kiralanamaz. (Kirada durumu zaman
    // aralığı hesabıyla ele alınır: bugün kirada olan ekipman yarın müsait olabilir.)
    private static bool KiralanabilirDurumda(EkipmanDurumu durum) =>
        durum is not (EkipmanDurumu.Bakimda or EkipmanDurumu.HizmetDisi);

    private static void AralikKontrol(DateTime baslangic, DateTime bitis)
    {
        if (bitis <= baslangic)
            throw new DogrulamaException("Bitiş zamanı başlangıç zamanından sonra olmalıdır.");
    }
}
