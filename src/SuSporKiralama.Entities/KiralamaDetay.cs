using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Entities;

/// <summary>Kiralamadaki tek bir ekipman satırı.</summary>
public class KiralamaDetay : BaseEntity
{
    private decimal _uygulananBirimUcret;
    private decimal? _hesaplananUcret;

    protected KiralamaDetay() { }

    /// <summary>Ekipmanın o anki fiyatı kopyalanır; sonradan fiyat değişse de bu kiralama etkilenmez.</summary>
    public KiralamaDetay(Ekipman ekipman)
    {
        Ekipman = ekipman;
        UygulananBirimUcret = ekipman.BirimUcret;
    }

    public int KiralamaId { get; set; }
    public Kiralama Kiralama { get; set; } = null!;

    public int EkipmanId { get; set; }
    public Ekipman Ekipman { get; set; } = null!;

    public decimal UygulananBirimUcret
    {
        get => _uygulananBirimUcret;
        set => _uygulananBirimUcret = Dogrula.NegatifOlamaz(value, nameof(UygulananBirimUcret));
    }

    // İade anında Ekipman.UcretHesapla(sure, UygulananBirimUcret) ile doldurulur.
    public decimal? HesaplananUcret
    {
        get => _hesaplananUcret;
        set => _hesaplananUcret = value is null ? null : Dogrula.NegatifOlamaz(value.Value, nameof(HesaplananUcret));
    }

    public ICollection<HasarKaydi> HasarKayitlari { get; set; } = new List<HasarKaydi>();
}
