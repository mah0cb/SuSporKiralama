using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Entities;

public class Kano : Ekipman
{
    private const int BlokSaat = 4;
    private int _kisiKapasitesi;

    protected Kano() { }

    public Kano(string kod, string marka, string model, decimal blokUcreti, decimal depozitoTutari, int kisiKapasitesi)
        : base(kod, marka, model, blokUcreti, depozitoTutari)
    {
        KisiKapasitesi = kisiKapasitesi;
    }

    public int KisiKapasitesi
    {
        get => _kisiKapasitesi;
        set => _kisiKapasitesi = Dogrula.PozitifOlmali(value, nameof(KisiKapasitesi));
    }

    /// <summary>4 saatlik blok ücreti: süre 4 saatlik bloklara yukarı yuvarlanır (en az 1 blok).</summary>
    public override decimal UcretHesapla(TimeSpan sure, decimal birimUcret)
    {
        GirdileriDogrula(sure, birimUcret);
        var blok = Math.Max(1, (int)Math.Ceiling(sure.TotalHours / BlokSaat));
        return blok * birimUcret;
    }
}
