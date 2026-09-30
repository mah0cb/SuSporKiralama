using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Tests;

/// <summary>Kiralama entity'sinin durum makinesi ve depozito hesabı (veritabanı gerekmez).</summary>
public class KiralamaTestleri
{
    private static readonly DateTime Baslangic = new(2026, 7, 1, 10, 0, 0);

    private static SupBoard Sup(decimal depozito = 1500m) =>
        new("SUP-001", "Aqua Marina", "Beast", 150m, depozito, 320, 120, SupBoardTipi.Sisme);

    private static Kiralama YeniKiralama(params Ekipman[] ekipmanlar)
    {
        var kiralama = new Kiralama(1, 1, Baslangic, Baslangic.AddHours(2));
        foreach (var ekipman in ekipmanlar.DefaultIfEmpty(Sup()))
            kiralama.Detaylar.Add(new KiralamaDetay(ekipman));
        return kiralama;
    }

    private static Kiralama AktifKiralama(params Ekipman[] ekipmanlar)
    {
        var kiralama = YeniKiralama(ekipmanlar);
        kiralama.TeslimEt(Baslangic);
        return kiralama;
    }

    // --- Geçerli geçişler ---

    [Fact]
    public void YeniKiralama_RezerveVeDepozitoAlinmamis()
    {
        var kiralama = YeniKiralama();

        Assert.Equal(KiralamaDurumu.Rezerve, kiralama.Durum);
        Assert.Equal(DepozitoDurumu.Alinmadi, kiralama.DepozitoDurumu);
    }

    [Fact]
    public void TeslimEt_AktifOlurDepozitoAlinirEkipmanKirada()
    {
        var sup = Sup(1500m);
        var yelek = new CanYelegi("YLK-001", "Decathlon", "Itiwit", 50m, 200m, Beden.M);
        var kiralama = YeniKiralama(sup, yelek);
        var teslimAni = Baslangic.AddMinutes(-15); // erken teslim

        kiralama.TeslimEt(teslimAni);

        Assert.Equal(KiralamaDurumu.Aktif, kiralama.Durum);
        Assert.Equal(teslimAni, kiralama.BaslangicZamani);
        Assert.Equal(1700m, kiralama.DepozitoTutari);
        Assert.Equal(DepozitoDurumu.Alindi, kiralama.DepozitoDurumu);
        Assert.Equal(EkipmanDurumu.Kirada, sup.Durum);
        Assert.Equal(EkipmanDurumu.Kirada, yelek.Durum);
    }

    [Fact]
    public void Tamamla_UcretGercekSureyleHesaplanirEkipmanMusait()
    {
        var sup = Sup();
        var kiralama = AktifKiralama(sup);

        kiralama.Tamamla(Baslangic.AddMinutes(150)); // 2,5 saat → 3 saat

        Assert.Equal(KiralamaDurumu.Tamamlandi, kiralama.Durum);
        Assert.Equal(450m, kiralama.ToplamUcret);
        Assert.Equal(EkipmanDurumu.Musait, sup.Durum);
        Assert.Equal(DepozitoDurumu.IadeEdildi, kiralama.DepozitoDurumu);
    }

    [Fact]
    public void IptalEt_RezerveKiralama_IptalEdilir()
    {
        var kiralama = YeniKiralama();

        kiralama.IptalEt();

        Assert.Equal(KiralamaDurumu.IptalEdildi, kiralama.Durum);
    }

    // --- Geçersiz geçişler ---

    [Fact]
    public void IptalEt_AktifKiralama_Hata()
    {
        var kiralama = AktifKiralama();

        Assert.Throws<InvalidOperationException>(kiralama.IptalEt);
        Assert.Equal(KiralamaDurumu.Aktif, kiralama.Durum);
    }

    [Fact]
    public void Tamamla_RezerveKiralama_Hata()
    {
        Assert.Throws<InvalidOperationException>(() => YeniKiralama().Tamamla(Baslangic.AddHours(1)));
    }

    [Fact]
    public void TeslimEt_TamamlanmisKiralama_Hata()
    {
        var kiralama = AktifKiralama();
        kiralama.Tamamla(Baslangic.AddHours(1));

        Assert.Throws<InvalidOperationException>(() => kiralama.TeslimEt(Baslangic.AddHours(1)));
    }

    [Fact]
    public void TeslimEt_IptalEdilmisKiralama_Hata()
    {
        var kiralama = YeniKiralama();
        kiralama.IptalEt();

        Assert.Throws<InvalidOperationException>(() => kiralama.TeslimEt(Baslangic));
    }

    [Fact]
    public void TeslimEt_PlanlananBitisGecmis_Hata()
    {
        var kiralama = YeniKiralama();

        Assert.Throws<InvalidOperationException>(() => kiralama.TeslimEt(Baslangic.AddHours(2)));
        Assert.Equal(KiralamaDurumu.Rezerve, kiralama.Durum);
    }

    [Fact]
    public void Olustur_BitisBaslangictanOnce_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Kiralama(1, 1, Baslangic, Baslangic));
    }

    // --- Gecikme ---

    [Fact]
    public void GecikmeSuresi_AktifteSimdiyeGoreTamamlanandaIadeyeGore()
    {
        var kiralama = AktifKiralama(); // planlanan bitiş 12:00

        Assert.Equal(TimeSpan.Zero, kiralama.GecikmeSuresi(Baslangic.AddHours(1)));
        Assert.Equal(TimeSpan.FromMinutes(30), kiralama.GecikmeSuresi(Baslangic.AddMinutes(150)));

        kiralama.Tamamla(Baslangic.AddMinutes(140));
        Assert.Equal(TimeSpan.FromMinutes(20), kiralama.GecikmeSuresi(Baslangic.AddDays(1)));
    }

    // --- Depozito mahsubu ---

    [Theory]
    [InlineData(1500, 0, 0, 1500, 0)]      // hasar yok: depozitonun tamamı iade
    [InlineData(1500, 300, 300, 1200, 0)]  // hasar < depozito: kısmen iade
    [InlineData(1500, 1500, 1500, 0, 0)]   // hasar = depozito: iade yok
    [InlineData(1500, 2000, 1500, 0, 500)] // hasar > depozito: aşan 500 ek ödeme
    public void DepozitoMahsupHesapla(decimal depozito, decimal hasar, decimal mahsup, decimal iade, decimal ekOdeme)
    {
        var sonuc = Kiralama.DepozitoMahsupHesapla(depozito, hasar);

        Assert.Equal((mahsup, iade, ekOdeme), sonuc);
    }

    [Theory]
    [InlineData(0, DepozitoDurumu.IadeEdildi)]
    [InlineData(300, DepozitoDurumu.KismenIadeEdildi)]
    [InlineData(1500, DepozitoDurumu.MahsupEdildi)]
    [InlineData(2000, DepozitoDurumu.MahsupEdildi)]
    public void Tamamla_HasaraGoreDepozitoDurumu(decimal hasarBedeli, DepozitoDurumu beklenen)
    {
        var sup = Sup(1500m);
        var kiralama = AktifKiralama(sup);
        if (hasarBedeli > 0)
            kiralama.Detaylar.First().HasarKayitlari.Add(new HasarKaydi(0, "Çizik", hasarBedeli));

        kiralama.Tamamla(Baslangic.AddHours(1));

        Assert.Equal(beklenen, kiralama.DepozitoDurumu);
        Assert.Equal(Math.Min(1500m, hasarBedeli), kiralama.DepozitoMahsupTutari);
        Assert.Equal(150m + hasarBedeli, kiralama.ToplamUcret);
        // Ödenecek = kiralama ücreti + depozitoyu aşan hasar
        Assert.Equal(150m + Math.Max(0, hasarBedeli - 1500m), kiralama.KalanBorc);
        Assert.Equal(hasarBedeli > 0 ? EkipmanDurumu.Bakimda : EkipmanDurumu.Musait, sup.Durum);
    }
}
