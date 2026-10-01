namespace SuSporKiralama.Business.Raporlar;

// Rapor sonuçları: yalnızca veri taşıyan, değiştirilemeyen record'lar (UI'da grid/grafik kaynağı olur).

public record GunlukGelir(DateOnly Tarih, decimal Tutar);

/// <summary>Ay: 1–12.</summary>
public record AylikGelir(int Ay, decimal Tutar);

public record DashboardOzeti(
    decimal BugunkuGelir,
    int AktifKiralamaSayisi,
    int GecikmisKiralamaSayisi,
    int BugunkuRezervasyonSayisi,
    int BakimdakiEkipmanSayisi,
    int MusaitEkipmanSayisi);
