using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Entities;

public class Odeme : BaseEntity
{
    private decimal _tutar;

    protected Odeme() { }

    public Odeme(int kiralamaId, decimal tutar, OdemeTipi odemeTipi, DateTime odemeTarihi)
    {
        KiralamaId = kiralamaId;
        Tutar = tutar;
        OdemeTipi = odemeTipi;
        OdemeTarihi = odemeTarihi;
    }

    public int KiralamaId { get; set; }
    public Kiralama Kiralama { get; set; } = null!;

    public decimal Tutar
    {
        get => _tutar;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Ödeme tutarı sıfırdan büyük olmalıdır.");
            _tutar = value;
        }
    }

    public OdemeTipi OdemeTipi { get; set; }

    public DateTime OdemeTarihi { get; set; }

    public string? Aciklama { get; set; }
}
