namespace SuSporKiralama.Business.Raporlar;

// Rapor sonuçları: yalnızca veri taşıyan, değiştirilemeyen record'lar (UI'da grid/grafik kaynağı olur).

public record GunlukGelir(DateOnly Tarih, decimal Tutar);

/// <summary>Ay: 1–12.</summary>
public record AylikGelir(int Ay, decimal Tutar);

/// <summary>Tur: ekipman türü (SupBoard, Kano, CanYelegi). KiralamaUcreti: hasar bedeli hariç, yalnızca iadesi alınmış kiralamalardan.</summary>
public record EkipmanKiralamaIstatistigi(string Kod, string Tur, int KiralanmaSayisi, TimeSpan ToplamSure, decimal KiralamaUcreti);

/// <summary>Oran 0–1 arası: KiralananSaat / (EkipmanSayisi × AcikSaat).</summary>
public record DolulukOrani(string Tur, int EkipmanSayisi, double KiralananSaat, double AcikSaat, double Oran);

public record DashboardOzeti(
    decimal BugunkuGelir,
    int AktifKiralamaSayisi,
    int GecikmisKiralamaSayisi,
    int BugunkuRezervasyonSayisi,
    int BakimdakiEkipmanSayisi,
    int MusaitEkipmanSayisi);
