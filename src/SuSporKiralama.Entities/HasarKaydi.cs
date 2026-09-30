using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Entities;

/// <summary>İade sırasında bir ekipmanda tespit edilen hasar.</summary>
public class HasarKaydi : BaseEntity
{
    private string _aciklama = null!;
    private decimal _hasarBedeli;

    protected HasarKaydi() { }

    public HasarKaydi(int kiralamaDetayId, string aciklama, decimal hasarBedeli)
    {
        KiralamaDetayId = kiralamaDetayId;
        Aciklama = aciklama;
        HasarBedeli = hasarBedeli;
    }

    public int KiralamaDetayId { get; set; }
    public KiralamaDetay KiralamaDetay { get; set; } = null!;

    public string Aciklama
    {
        get => _aciklama;
        set => _aciklama = Dogrula.BosOlamaz(value, nameof(Aciklama));
    }

    public decimal HasarBedeli
    {
        get => _hasarBedeli;
        set => _hasarBedeli = Dogrula.NegatifOlamaz(value, nameof(HasarBedeli));
    }

    public DateTime KayitTarihi { get; set; } = DateTime.Now;
}
