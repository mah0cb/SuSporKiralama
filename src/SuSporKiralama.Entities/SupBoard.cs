using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Entities;

public class SupBoard : Ekipman
{
    private int _uzunlukCm;
    private int _maxTasimaKg;

    protected SupBoard() { }

    public SupBoard(string kod, string marka, string model, decimal saatlikUcret, decimal depozitoTutari,
        int uzunlukCm, int maxTasimaKg, SupBoardTipi tip)
        : base(kod, marka, model, saatlikUcret, depozitoTutari)
    {
        UzunlukCm = uzunlukCm;
        MaxTasimaKg = maxTasimaKg;
        Tip = tip;
    }

    public int UzunlukCm
    {
        get => _uzunlukCm;
        set => _uzunlukCm = Dogrula.PozitifOlmali(value, nameof(UzunlukCm));
    }

    public int MaxTasimaKg
    {
        get => _maxTasimaKg;
        set => _maxTasimaKg = Dogrula.PozitifOlmali(value, nameof(MaxTasimaKg));
    }

    public SupBoardTipi Tip { get; set; }

    /// <summary>Saatlik ücret: süre tam saate yukarı yuvarlanır, en az 1 saat ücretlenir.</summary>
    public override decimal UcretHesapla(TimeSpan sure, decimal birimUcret)
    {
        GirdileriDogrula(sure, birimUcret);
        var saat = Math.Max(1, (int)Math.Ceiling(sure.TotalHours));
        return saat * birimUcret;
    }
}
