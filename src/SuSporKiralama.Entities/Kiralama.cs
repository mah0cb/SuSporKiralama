using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Entities;

/// <summary>Bir müşterinin bir veya birden fazla ekipmanı kiraladığı işlem (başlık kaydı).</summary>
public class Kiralama : BaseEntity
{
    private DateTime _planlananBitisZamani;
    private decimal? _toplamUcret;

    protected Kiralama() { }

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

    public DateTime BaslangicZamani { get; set; }

    public DateTime PlanlananBitisZamani
    {
        get => _planlananBitisZamani;
        set
        {
            if (value <= BaslangicZamani)
                throw new ArgumentException("Planlanan bitiş zamanı başlangıç zamanından sonra olmalıdır.");
            _planlananBitisZamani = value;
        }
    }

    // İade yapılınca dolar; aktif kiralamada null'dır.
    public DateTime? GercekBitisZamani { get; set; }

    public KiralamaDurumu Durum { get; set; } = KiralamaDurumu.Aktif;

    // İade anında hesaplanır; o ana kadar null'dır.
    public decimal? ToplamUcret
    {
        get => _toplamUcret;
        set => _toplamUcret = value is null ? null : Dogrula.NegatifOlamaz(value.Value, nameof(ToplamUcret));
    }

    public ICollection<KiralamaDetay> Detaylar { get; set; } = new List<KiralamaDetay>();
    public ICollection<Odeme> Odemeler { get; set; } = new List<Odeme>();
}
