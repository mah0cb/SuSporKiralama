namespace SuSporKiralama.Entities.Soyut;

/// <summary>
/// Kiralanabilir ekipmanların soyut atası. SupBoard, Kano ve CanYelegi bu sınıftan türer.
/// Her tür ücretini farklı hesapladığı için UcretHesapla soyuttur (çok biçimlilik).
/// Veritabanında TPH ile tek tabloda (Ekipmanlar) tutulur.
/// </summary>
public abstract class Ekipman : BaseEntity
{
    // Setter'lardaki doğrulama için backing field'lar. EF Core veritabanından okurken
    // bu alanlara doğrudan yazar, böylece doğrulama sadece bizim atamalarımızda çalışır.
    private string _kod = null!;
    private string _marka = null!;
    private string _model = null!;
    private decimal _birimUcret;
    private decimal _depozitoTutari;

    // EF Core'un nesne oluşturabilmesi için parametresiz kurucu (dışarıdan kullanılmaz).
    protected Ekipman() { }

    protected Ekipman(string kod, string marka, string model, decimal birimUcret, decimal depozitoTutari)
    {
        Kod = kod;
        Marka = marka;
        Model = model;
        BirimUcret = birimUcret;
        DepozitoTutari = depozitoTutari;
    }

    /// <summary>Benzersiz ekipman kodu, ör. SUP-001.</summary>
    public string Kod
    {
        get => _kod;
        set => _kod = Dogrula.BosOlamaz(value, nameof(Kod)).ToUpperInvariant();
    }

    public string Marka
    {
        get => _marka;
        set => _marka = Dogrula.BosOlamaz(value, nameof(Marka));
    }

    public string Model
    {
        get => _model;
        set => _model = Dogrula.BosOlamaz(value, nameof(Model));
    }

    public EkipmanDurumu Durum { get; set; } = EkipmanDurumu.Musait;

    /// <summary>Anlamı alt sınıfa göre değişir: SUP'ta saatlik, kanoda 4 saatlik blok, yelekte kiralama başı.</summary>
    public decimal BirimUcret
    {
        get => _birimUcret;
        set => _birimUcret = Dogrula.NegatifOlamaz(value, nameof(BirimUcret));
    }

    public decimal DepozitoTutari
    {
        get => _depozitoTutari;
        set => _depozitoTutari = Dogrula.NegatifOlamaz(value, nameof(DepozitoTutari));
    }

    /// <summary>Güncel BirimUcret ile ücret hesaplar (kısayol).</summary>
    public decimal UcretHesapla(TimeSpan sure) => UcretHesapla(sure, BirimUcret);

    /// <summary>
    /// Verilen birim ücretle ücret hesaplar. İade anında kiralama sırasında kopyalanan
    /// fiyat (KiralamaDetay.UygulananBirimUcret) verilir; böylece sonradan yapılan
    /// fiyat değişiklikleri eski kiralamaları etkilemez.
    /// </summary>
    public abstract decimal UcretHesapla(TimeSpan sure, decimal birimUcret);

    // Alt sınıfların ortak ön kontrolü.
    protected static void GirdileriDogrula(TimeSpan sure, decimal birimUcret)
    {
        if (sure < TimeSpan.Zero)
            throw new ArgumentException("Kiralama süresi negatif olamaz.", nameof(sure));
        Dogrula.NegatifOlamaz(birimUcret, nameof(birimUcret));
    }
}
