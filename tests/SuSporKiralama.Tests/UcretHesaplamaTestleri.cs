using SuSporKiralama.Entities;

namespace SuSporKiralama.Tests;

/// <summary>
/// Polimorfik UcretHesapla'nın sınır değerleri. Süre dakika olarak verilir;
/// birim ücret kolay okunsun diye 100 TL alınır.
/// </summary>
public class UcretHesaplamaTestleri
{
    private const decimal BirimUcret = 100m;

    // SupBoard: saatlik, tam saate yukarı yuvarlanır, en az 1 saat.
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(60, 1)]
    [InlineData(61, 2)]
    [InlineData(120, 2)]
    [InlineData(121, 3)]
    public void SupBoard_SaatYukariYuvarlanir(int dakika, int beklenenSaat)
    {
        var sup = new SupBoard("SUP-001", "Aqua Marina", "Beast", BirimUcret, 1500m, 320, 120, SupBoardTipi.Sisme);

        Assert.Equal(beklenenSaat * BirimUcret, sup.UcretHesapla(TimeSpan.FromMinutes(dakika)));
    }

    // Kano: 4 saatlik bloklar, yukarı yuvarlanır, en az 1 blok.
    [Theory]
    [InlineData(0, 1)]
    [InlineData(60, 1)]
    [InlineData(240, 1)]
    [InlineData(241, 2)]
    [InlineData(480, 2)]
    [InlineData(481, 3)]
    public void Kano_DortSaatlikBlokYukariYuvarlanir(int dakika, int beklenenBlok)
    {
        var kano = new Kano("KANO-001", "Pelican", "Argo", BirimUcret, 2500m, 1);

        Assert.Equal(beklenenBlok * BirimUcret, kano.UcretHesapla(TimeSpan.FromMinutes(dakika)));
    }

    // Can yeleği: süreden bağımsız sabit ücret.
    [Theory]
    [InlineData(0)]
    [InlineData(61)]
    [InlineData(300)]
    [InlineData(3 * 24 * 60)]
    public void CanYelegi_SuredenBagimsizSabit(int dakika)
    {
        var yelek = new CanYelegi("YLK-001", "Decathlon", "Itiwit", BirimUcret, 200m, Beden.M);

        Assert.Equal(BirimUcret, yelek.UcretHesapla(TimeSpan.FromMinutes(dakika)));
    }

    [Fact]
    public void UcretHesapla_KiralamaAnindakiFiyatKullanilir()
    {
        var sup = new SupBoard("SUP-001", "Aqua Marina", "Beast", 150m, 1500m, 320, 120, SupBoardTipi.Sisme);

        // Fiyat sonradan 200'e çıksa da kiralamadaki 150 ile hesaplanır.
        sup.BirimUcret = 200m;

        Assert.Equal(300m, sup.UcretHesapla(TimeSpan.FromHours(2), 150m));
    }

    [Fact]
    public void UcretHesapla_NegatifSure_ArgumentException()
    {
        var sup = new SupBoard("SUP-001", "Aqua Marina", "Beast", BirimUcret, 1500m, 320, 120, SupBoardTipi.Sisme);

        var hata = Assert.Throws<ArgumentException>(() => sup.UcretHesapla(TimeSpan.FromMinutes(-1)));
        Assert.Equal("Kiralama süresi negatif olamaz.", hata.Message);
    }
}
