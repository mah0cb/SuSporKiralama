using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Entities;

public class Personel : BaseEntity
{
    private string _adSoyad = null!;
    private string _kullaniciAdi = null!;
    private string _sifreHash = null!;

    protected Personel() { }

    public Personel(string adSoyad, string kullaniciAdi, string sifreHash, Rol rol)
    {
        AdSoyad = adSoyad;
        KullaniciAdi = kullaniciAdi;
        SifreHash = sifreHash;
        Rol = rol;
    }

    public string AdSoyad
    {
        get => _adSoyad;
        set => _adSoyad = Dogrula.BosOlamaz(value, nameof(AdSoyad));
    }

    /// <summary>Benzersiz; giriş sırasında büyük/küçük harf sorunu olmasın diye küçük harfe çevrilir.</summary>
    public string KullaniciAdi
    {
        get => _kullaniciAdi;
        set => _kullaniciAdi = Dogrula.BosOlamaz(value, nameof(KullaniciAdi)).ToLowerInvariant();
    }

    /// <summary>Düz şifre asla saklanmaz; sadece PBKDF2 özeti tutulur.</summary>
    public string SifreHash
    {
        get => _sifreHash;
        set => _sifreHash = Dogrula.BosOlamaz(value, nameof(SifreHash));
    }

    public Rol Rol { get; set; }

    // Personel silinmez, pasife alınır (geçmiş kiralamalarda kimin işlem yaptığı korunur).
    public bool AktifMi { get; set; } = true;

    public ICollection<Kiralama> Kiralamalar { get; set; } = new List<Kiralama>();
}
