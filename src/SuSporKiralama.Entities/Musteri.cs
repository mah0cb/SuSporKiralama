using System.Net.Mail;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Entities;

public class Musteri : BaseEntity
{
    private string _ad = null!;
    private string _soyad = null!;
    private string _telefon = null!;
    private string? _eposta;

    protected Musteri() { }

    public Musteri(string ad, string soyad, string telefon, DeneyimSeviyesi deneyimSeviyesi)
    {
        Ad = ad;
        Soyad = soyad;
        Telefon = telefon;
        DeneyimSeviyesi = deneyimSeviyesi;
    }

    public string Ad
    {
        get => _ad;
        set => _ad = Dogrula.BosOlamaz(value, nameof(Ad));
    }

    public string Soyad
    {
        get => _soyad;
        set => _soyad = Dogrula.BosOlamaz(value, nameof(Soyad));
    }

    /// <summary>Benzersiz; müşteriyi bulmak için kullanılır.</summary>
    public string Telefon
    {
        get => _telefon;
        set => _telefon = Dogrula.BosOlamaz(value, nameof(Telefon));
    }

    /// <summary>Opsiyonel; girilirse geçerli bir e-posta adresi olmalıdır.</summary>
    public string? Eposta
    {
        get => _eposta;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) { _eposta = null; return; }
            if (!MailAddress.TryCreate(value.Trim(), out _))
                throw new ArgumentException("Geçerli bir e-posta adresi giriniz.");
            _eposta = value.Trim();
        }
    }

    public DeneyimSeviyesi DeneyimSeviyesi { get; set; }

    public string? Notlar { get; set; }

    // Kiralama geçmişi olan müşteri silinmez, pasife alınır.
    public bool AktifMi { get; set; } = true;

    public string AdSoyad => $"{Ad} {Soyad}";

    public ICollection<Kiralama> Kiralamalar { get; set; } = new List<Kiralama>();
}
