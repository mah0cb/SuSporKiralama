using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Entities;

public class CanYelegi : Ekipman
{
    protected CanYelegi() { }

    public CanYelegi(string kod, string marka, string model, decimal sabitUcret, decimal depozitoTutari, Beden beden)
        : base(kod, marka, model, sabitUcret, depozitoTutari)
    {
        Beden = beden;
    }

    public Beden Beden { get; set; }

    /// <summary>Kiralama başına sabit ücret; süreden bağımsızdır.</summary>
    public override decimal UcretHesapla(TimeSpan sure, decimal birimUcret)
    {
        GirdileriDogrula(sure, birimUcret);
        return birimUcret;
    }
}
