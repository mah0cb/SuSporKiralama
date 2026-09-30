using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.DataAccess.Repository;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Business.Servisler;

/// <summary>
/// Kiralama iş akışları. CrudServisiTemel'den türemez: temel sınıfın public Ekle/Guncelle/Sil
/// metotları kiralamanın serbestçe silinmesine veya durum kuralları atlanarak güncellenmesine
/// izin verirdi. Durum geçiş kuralları Kiralama entity'sinde, veritabanı gerektiren kurallar
/// (müsaitlik, müşteri/personel geçerliliği) burada.
///
/// Atomiklik: her metot önce tüm kontrolleri yapar, sonra nesneleri değiştirir ve tek bir
/// SaveChanges çağırır. Böylece kural hatasında bellekte de veritabanında da değişiklik kalmaz.
/// </summary>
public class KiralamaServisi(
    IRepository<Kiralama> kiralamaRepository,
    IRepository<Musteri> musteriRepository,
    IRepository<Personel> personelRepository,
    IRepository<Ekipman> ekipmanRepository,
    IMusaitlikServisi musaitlikServisi,
    TimeProvider zaman) : IKiralamaServisi
{
    // Kiralama ile birlikte yüklenen ilişkiler (iade ve ödeme hesabı bunlara ihtiyaç duyar).
    private static readonly string[] Iliskiler =
    [
        nameof(Kiralama.Musteri),
        $"{nameof(Kiralama.Detaylar)}.{nameof(KiralamaDetay.Ekipman)}",
        $"{nameof(Kiralama.Detaylar)}.{nameof(KiralamaDetay.HasarKayitlari)}",
        nameof(Kiralama.Odemeler)
    ];

    private DateTime Simdi => zaman.GetLocalNow().DateTime;

    public Kiralama IdIleGetir(int id) =>
        kiralamaRepository.Find(k => k.Id == id, Iliskiler).SingleOrDefault()
        ?? throw new KayitBulunamadiException("Kiralama", id);

    // --- Oluşturma ---

    public Kiralama RezervasyonOlustur(int musteriId, int personelId, IEnumerable<int> ekipmanIdleri,
        DateTime baslangic, DateTime planlananBitis)
    {
        if (baslangic <= Simdi)
            throw new DogrulamaException("Rezervasyonun başlangıç zamanı gelecekte olmalıdır.");

        var kiralama = YeniKiralama(musteriId, personelId, ekipmanIdleri, baslangic, planlananBitis);
        kiralamaRepository.Add(kiralama);
        kiralamaRepository.SaveChanges();
        return kiralama;
    }

    public Kiralama KiralamaBaslat(int musteriId, int personelId, IEnumerable<int> ekipmanIdleri, DateTime planlananBitis)
    {
        var simdi = Simdi;
        var kiralama = YeniKiralama(musteriId, personelId, ekipmanIdleri, simdi, planlananBitis);
        kiralama.TeslimEt(simdi); // kapıdan kiralama = oluştur + hemen teslim et

        kiralamaRepository.Add(kiralama);
        kiralamaRepository.SaveChanges();
        return kiralama;
    }

    // --- Durum geçişleri ---

    public void TeslimEt(int kiralamaId)
    {
        var kiralama = IdIleGetir(kiralamaId);
        var simdi = Simdi;

        // Rezervasyondan bu yana ekipman bakıma alınmış veya gecikmiş bir kiralama yüzünden
        // hâlâ dışarıda olabilir. Teslim mümkün değilse (durum/süre) hatayı entity verir.
        if (kiralama.Durum == KiralamaDurumu.Rezerve && simdi < kiralama.PlanlananBitisZamani)
            MusaitlikKontrol(kiralama.Detaylar.Select(d => d.Ekipman), simdi, kiralama.PlanlananBitisZamani, kiralama.Id);

        KuralIle(() => kiralama.TeslimEt(simdi));
        kiralamaRepository.SaveChanges();
    }

    public void IadeAl(int kiralamaId, IEnumerable<HasarBilgisi>? hasarlar = null)
    {
        var kiralama = IdIleGetir(kiralamaId);
        var simdi = Simdi;

        // Hasarlar detaylara eklenmeden önce kontrol edilir; aksi halde hata durumunda eklenen
        // hasar kayıtları bellekte kalırdı.
        if (kiralama.Durum != KiralamaDurumu.Aktif)
            throw new IslemYapilamazException($"{kiralama.Durum} durumundaki kiralamanın iadesi alınamaz.");

        var eklenecekler = new List<(KiralamaDetay Detay, HasarKaydi Hasar)>();
        foreach (var bilgi in hasarlar ?? [])
        {
            var detay = kiralama.Detaylar.FirstOrDefault(d => d.Id == bilgi.KiralamaDetayId)
                ?? throw new DogrulamaException($"Hasar bildirilen satır (Id: {bilgi.KiralamaDetayId}) bu kiralamaya ait değil.");

            HasarKaydi hasar = null!;
            KuralIle(() => hasar = new HasarKaydi(detay.Id, bilgi.Aciklama, bilgi.Bedel) { KayitTarihi = simdi });
            eklenecekler.Add((detay, hasar));
        }

        foreach (var (detay, hasar) in eklenecekler)
            detay.HasarKayitlari.Add(hasar);
        kiralama.Tamamla(simdi); // ücret, depozito mahsubu ve ekipman durumları

        kiralamaRepository.SaveChanges();
    }

    public void IptalEt(int kiralamaId)
    {
        var kiralama = IdIleGetir(kiralamaId);
        KuralIle(kiralama.IptalEt);
        kiralamaRepository.SaveChanges();
    }

    // --- Ödeme ---

    public Odeme OdemeEkle(int kiralamaId, decimal tutar, OdemeTipi odemeTipi, string? aciklama = null)
    {
        var kiralama = IdIleGetir(kiralamaId);

        // Tutar iade anında kesinleşir; öncesinde fazla ödemenin sınırı belli olmaz.
        if (kiralama.Durum != KiralamaDurumu.Tamamlandi)
            throw new IslemYapilamazException("Ödeme yalnızca iadesi alınmış (tamamlanmış) kiralamaya eklenebilir.");

        Odeme odeme = null!;
        KuralIle(() => odeme = new Odeme(kiralama.Id, tutar, odemeTipi, Simdi) { Aciklama = aciklama });

        if (tutar > kiralama.KalanBorc)
            throw new IslemYapilamazException(
                $"Ödeme tutarı ({tutar:N2} TL) kalan borcu ({kiralama.KalanBorc:N2} TL) aşamaz.");

        kiralama.Odemeler.Add(odeme);
        kiralamaRepository.SaveChanges();
        return odeme;
    }

    // --- Sorgular ---

    public List<Kiralama> AktifKiralamalar() =>
        kiralamaRepository.Find(k => k.Durum == KiralamaDurumu.Aktif, Iliskiler)
            .OrderBy(k => k.PlanlananBitisZamani)
            .ToList();

    public List<Kiralama> GecikmisKiralamalar()
    {
        var simdi = Simdi;
        return kiralamaRepository.Find(k => k.Durum == KiralamaDurumu.Aktif && k.PlanlananBitisZamani < simdi, Iliskiler)
            .OrderBy(k => k.PlanlananBitisZamani)
            .ToList();
    }

    public List<Kiralama> MusteriGecmisi(int musteriId) =>
        kiralamaRepository.Find(k => k.MusteriId == musteriId, Iliskiler)
            .OrderByDescending(k => k.BaslangicZamani)
            .ToList();

    public List<Kiralama> BugunkuRezervasyonlar()
    {
        var bugun = Simdi.Date;
        var yarin = bugun.AddDays(1);
        return kiralamaRepository.Find(k => k.Durum == KiralamaDurumu.Rezerve
                                            && k.BaslangicZamani >= bugun && k.BaslangicZamani < yarin, Iliskiler)
            .OrderBy(k => k.BaslangicZamani)
            .ToList();
    }

    // --- Yardımcılar ---

    /// <summary>Tüm kontrollerden geçmiş, henüz context'e eklenmemiş Rezerve kiralama oluşturur.</summary>
    private Kiralama YeniKiralama(int musteriId, int personelId, IEnumerable<int> ekipmanIdleri,
        DateTime baslangic, DateTime planlananBitis)
    {
        if (planlananBitis <= baslangic)
            throw new DogrulamaException("Planlanan bitiş zamanı başlangıç zamanından sonra olmalıdır.");

        var musteri = musteriRepository.GetById(musteriId) ?? throw new KayitBulunamadiException("Müşteri", musteriId);
        if (!musteri.AktifMi)
            throw new IslemYapilamazException($"{musteri.AdSoyad} pasif bir müşteri olduğu için kiralama yapılamaz.");

        var personel = personelRepository.GetById(personelId) ?? throw new KayitBulunamadiException("Personel", personelId);
        if (!personel.AktifMi)
            throw new IslemYapilamazException($"{personel.AdSoyad} pasif bir personel olduğu için kiralama yapamaz.");

        var idler = ekipmanIdleri.ToList();
        if (idler.Count == 0)
            throw new DogrulamaException("Kiralama için en az bir ekipman seçilmelidir.");
        if (idler.Distinct().Count() != idler.Count)
            throw new DogrulamaException("Aynı ekipman bir kiralamaya iki kez eklenemez.");

        var ekipmanlar = idler
            .Select(id => ekipmanRepository.GetById(id) ?? throw new KayitBulunamadiException("Ekipman", id))
            .ToList();
        MusaitlikKontrol(ekipmanlar, baslangic, planlananBitis, haricKiralamaId: null);

        var kiralama = new Kiralama(musteriId, personelId, baslangic, planlananBitis) { OlusturmaTarihi = Simdi };
        foreach (var ekipman in ekipmanlar)
            kiralama.Detaylar.Add(new KiralamaDetay(ekipman)); // o anki fiyat kopyalanır
        return kiralama;
    }

    // ponytail: ekipman başına bir sorgu; kiralamada birkaç ekipman olduğu için yeterli.
    private void MusaitlikKontrol(IEnumerable<Ekipman> ekipmanlar, DateTime baslangic, DateTime bitis, int? haricKiralamaId)
    {
        var musaitOlmayanlar = ekipmanlar
            .Where(e => !musaitlikServisi.MusaitMi(e.Id, baslangic, bitis, haricKiralamaId))
            .Select(e => e.Kod)
            .ToList();

        if (musaitOlmayanlar.Count > 0)
            throw new IslemYapilamazException(
                $"Şu ekipmanlar istenen zaman aralığında müsait değil: {string.Join(", ", musaitOlmayanlar)}.");
    }

    /// <summary>
    /// Entity kurallarını iş kuralı exception'larına çevirir: geçersiz değer (ArgumentException)
    /// → DogrulamaException, geçersiz durum geçişi (InvalidOperationException) → IslemYapilamazException.
    /// </summary>
    private static void KuralIle(Action islem)
    {
        try
        {
            islem();
        }
        catch (ArgumentException ex)
        {
            throw new DogrulamaException(ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new IslemYapilamazException(ex.Message, ex);
        }
    }
}
