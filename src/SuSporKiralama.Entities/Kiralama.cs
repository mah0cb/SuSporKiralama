using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Entities;

/// <summary>
/// Bir müşterinin bir veya birden fazla ekipmanı kiraladığı işlem (başlık kaydı).
/// Durum yalnızca aşağıdaki metotlarla değişir (durum makinesi):
///   Rezerve --TeslimEt--> Aktif --Tamamla--> Tamamlandi
///   Rezerve --IptalEt--> IptalEdildi
/// Geçersiz bir geçişte InvalidOperationException fırlatılır. Kurallar entity'de durur;
/// böylece servis, seed veya ileride UI fark etmeksizin "Durum = ..." ile atlanamaz.
/// </summary>
public class Kiralama : BaseEntity
{
    private DateTime _planlananBitisZamani;

    protected Kiralama() { }

    /// <summary>Yeni kiralama Rezerve durumunda başlar. Kapıdan kiralamada hemen TeslimEt çağrılır.</summary>
    public Kiralama(int musteriId, int personelId, DateTime baslangicZamani, DateTime planlananBitisZamani)
    {
        MusteriId = musteriId;
        PersonelId = personelId;
        BaslangicZamani = baslangicZamani;
        PlanlananBitisZamani = planlananBitisZamani;
    }

    public int MusteriId { get; set; }
    public Musteri Musteri { get; set; } = null!;

    // Kiralamayı başlatan personel.
    public int PersonelId { get; set; }
    public Personel Personel { get; set; } = null!;

    // Rezervasyonda planlanan başlangıç; teslimde gerçek teslim anı olarak güncellenir.
    public DateTime BaslangicZamani { get; private set; }

    public DateTime PlanlananBitisZamani
    {
        get => _planlananBitisZamani;
        private set
        {
            if (value <= BaslangicZamani)
                throw new ArgumentException("Planlanan bitiş zamanı başlangıç zamanından sonra olmalıdır.");
            _planlananBitisZamani = value;
        }
    }

    // İade yapılınca dolar; aktif kiralamada null'dır.
    public DateTime? GercekBitisZamani { get; private set; }

    public KiralamaDurumu Durum { get; private set; } = KiralamaDurumu.Rezerve;

    // İade anında hesaplanır: kiralama ücretleri + hasar bedelleri. O ana kadar null'dır.
    public decimal? ToplamUcret { get; private set; }

    // Teslimde ekipmanların o anki depozito tutarlarının toplamı olarak alınır.
    public decimal DepozitoTutari { get; private set; }

    public DepozitoDurumu DepozitoDurumu { get; private set; } = DepozitoDurumu.Alinmadi;

    // Hasar bedelinin depozitodan kesilen kısmı; müşteriye DepozitoTutari - bu tutar iade edilir.
    public decimal DepozitoMahsupTutari { get; private set; }

    public ICollection<KiralamaDetay> Detaylar { get; set; } = new List<KiralamaDetay>();
    public ICollection<Odeme> Odemeler { get; set; } = new List<Odeme>();

    public decimal HasarBedeliToplami => Detaylar.SelectMany(d => d.HasarKayitlari).Sum(h => h.HasarBedeli);

    /// <summary>Müşterinin ödemesi gereken kalan tutar (Odemeler ve Detaylar yüklü olmalıdır).</summary>
    public decimal KalanBorc => (ToplamUcret ?? 0) - DepozitoMahsupTutari - Odemeler.Sum(o => o.Tutar);

    // --- Durum geçişleri ---

    /// <summary>Rezerve → Aktif: ekipmanlar müşteriye verilir, depozito alınır.</summary>
    public void TeslimEt(DateTime an)
    {
        DurumOlmali(KiralamaDurumu.Rezerve, "teslim edilemez");
        if (Detaylar.Count == 0)
            throw new InvalidOperationException("Ekipmanı olmayan kiralama teslim edilemez.");
        if (an >= PlanlananBitisZamani)
            throw new InvalidOperationException("Planlanan bitiş zamanı geçmiş bir rezervasyon teslim edilemez.");

        BaslangicZamani = an;
        DepozitoTutari = Detaylar.Sum(d => d.Ekipman.DepozitoTutari);
        DepozitoDurumu = DepozitoDurumu.Alindi;
        foreach (var detay in Detaylar)
            detay.Ekipman.Durum = EkipmanDurumu.Kirada;
        Durum = KiralamaDurumu.Aktif;
    }

    /// <summary>
    /// Aktif → Tamamlandi (iade). Hasar kayıtları bu metottan önce detaylara eklenmiş olmalıdır.
    /// Gecikme ayrıca cezalandırılmaz; ücret gerçek süre üzerinden hesaplanır.
    /// </summary>
    public void Tamamla(DateTime an)
    {
        DurumOlmali(KiralamaDurumu.Aktif, "tamamlanamaz");
        if (an < BaslangicZamani)
            throw new InvalidOperationException("İade zamanı teslim zamanından önce olamaz.");

        var sure = an - BaslangicZamani;
        foreach (var detay in Detaylar)
        {
            // Çok biçimlilik: her ekipman türü ücretini kendi kuralıyla hesaplar.
            detay.HesaplananUcret = detay.Ekipman.UcretHesapla(sure, detay.UygulananBirimUcret);
            detay.Ekipman.Durum = detay.HasarKayitlari.Count > 0 ? EkipmanDurumu.Bakimda : EkipmanDurumu.Musait;
        }

        var hasar = HasarBedeliToplami;
        var mahsup = DepozitoMahsupHesapla(DepozitoTutari, hasar);
        DepozitoMahsupTutari = mahsup.Mahsup;
        DepozitoDurumu = mahsup.Mahsup == 0 ? DepozitoDurumu.IadeEdildi
            : mahsup.IadeEdilecek > 0 ? DepozitoDurumu.KismenIadeEdildi
            : DepozitoDurumu.MahsupEdildi;

        ToplamUcret = Detaylar.Sum(d => d.HesaplananUcret!.Value) + hasar;
        GercekBitisZamani = an;
        Durum = KiralamaDurumu.Tamamlandi;
    }

    /// <summary>Rezerve → IptalEdildi. Aktif kiralama iptal edilemez, sadece iade alınabilir.</summary>
    public void IptalEt()
    {
        DurumOlmali(KiralamaDurumu.Rezerve, "iptal edilemez");
        Durum = KiralamaDurumu.IptalEdildi;
    }

    // --- Hesaplamalar ---

    /// <summary>Planlanan bitişten ne kadar geç kalındığı (aktifse şu ana göre); gecikme yoksa sıfır.</summary>
    public TimeSpan GecikmeSuresi(DateTime simdi)
    {
        var bitis = Durum switch
        {
            KiralamaDurumu.Aktif => simdi,
            KiralamaDurumu.Tamamlandi => GercekBitisZamani!.Value,
            _ => PlanlananBitisZamani
        };
        return bitis > PlanlananBitisZamani ? bitis - PlanlananBitisZamani : TimeSpan.Zero;
    }

    /// <summary>
    /// Hasar bedeli önce depozitodan düşülür (mahsup), kalan depozito iade edilir.
    /// Hasar depozitoyu aşarsa aşan kısım müşterinin ödeyeceği tutara eklenir (EkOdeme).
    /// </summary>
    public static (decimal Mahsup, decimal IadeEdilecek, decimal EkOdeme) DepozitoMahsupHesapla(decimal depozito, decimal hasarBedeli)
    {
        var mahsup = Math.Min(depozito, hasarBedeli);
        return (mahsup, depozito - mahsup, hasarBedeli - mahsup);
    }

    private void DurumOlmali(KiralamaDurumu beklenen, string islem)
    {
        if (Durum != beklenen)
            throw new InvalidOperationException($"{Durum} durumundaki kiralama {islem}.");
    }
}
