using Microsoft.Extensions.Time.Testing;
using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Tests;

public class MusaitlikServisiTestleri : IDisposable
{
    // "Şimdi" 1 Temmuz 10:00; testlerdeki saatler bu güne göre yazılır.
    private static readonly DateTime Bugun = new(2026, 7, 1);
    private static DateTime Saat(int saat, int dakika = 0) => Bugun.AddHours(saat).AddMinutes(dakika);

    private readonly TestVeritabani _db = new();
    private readonly FakeTimeProvider _zaman = new(new DateTimeOffset(Saat(10), TimeSpan.Zero));
    private readonly MusaitlikServisi _servis;
    private readonly Musteri _musteri;
    private readonly Personel _personel;
    private readonly SupBoard _sup;

    public MusaitlikServisiTestleri()
    {
        _servis = new MusaitlikServisi(_db.Repo<Ekipman>(), _db.Repo<KiralamaDetay>(), _zaman);
        _musteri = _db.MusteriEkle();
        _personel = _db.PersonelEkle();
        _sup = _db.SupEkle();
    }

    public void Dispose() => _db.Dispose();

    private Kiralama Kur(DateTime baslangic, DateTime bitis, KiralamaDurumu durum, params Ekipman[] ekipmanlar) =>
        _db.KiralamaKur(_musteri, _personel, baslangic, bitis, durum, ekipmanlar.Length > 0 ? ekipmanlar : [_sup]);

    [Fact]
    public void KiralamasiYok_Musait()
    {
        Assert.True(_servis.MusaitMi(_sup.Id, Saat(14), Saat(16)));
    }

    // Mevcut rezervasyon 14:00–16:00.
    [Theory]
    [InlineData(15, 0, 17, 0, false)]  // sonu taşıyor
    [InlineData(13, 0, 15, 0, false)]  // başı taşıyor
    [InlineData(14, 30, 15, 30, false)] // tamamen içinde
    [InlineData(13, 0, 17, 0, false)]  // tamamen kapsıyor
    [InlineData(14, 0, 16, 0, false)]  // aynı aralık
    [InlineData(16, 0, 18, 0, true)]   // uç uca: o 16:00'da bitiyor, bu 16:00'da başlıyor
    [InlineData(12, 0, 14, 0, true)]   // uç uca: bu 14:00'da bitiyor, o 14:00'da başlıyor
    [InlineData(17, 0, 18, 0, true)]   // tamamen sonra
    public void RezervasyonlaCakisma(int basSaat, int basDakika, int bitSaat, int bitDakika, bool musait)
    {
        Kur(Saat(14), Saat(16), KiralamaDurumu.Rezerve);

        Assert.Equal(musait, _servis.MusaitMi(_sup.Id, Saat(basSaat, basDakika), Saat(bitSaat, bitDakika)));
    }

    [Fact]
    public void AktifKiralamaIleCakisma_MusaitDegil()
    {
        Kur(Saat(9), Saat(12), KiralamaDurumu.Aktif);

        Assert.False(_servis.MusaitMi(_sup.Id, Saat(11), Saat(13)));
        Assert.True(_servis.MusaitMi(_sup.Id, Saat(12), Saat(13)));
    }

    [Fact]
    public void GecikmisAktifKiralama_SimdiyeKadarDoluSayilir()
    {
        // 06:00–08:00 planlanmış, şu an 10:00 ve hâlâ iade edilmedi.
        Kur(Saat(6), Saat(8), KiralamaDurumu.Aktif);

        Assert.False(_servis.MusaitMi(_sup.Id, Saat(9), Saat(11)));  // planlanan bitişten sonra ama şimdiden önce
        Assert.False(_servis.MusaitMi(_sup.Id, Saat(10), Saat(11))); // tam şimdi başlayan: ekipman hâlâ müşteride
        Assert.True(_servis.MusaitMi(_sup.Id, Saat(11), Saat(12)));  // gelecekteki aralık engellenmez
    }

    [Fact]
    public void GecikmisKiralama_ZamanIlerledikceDoluAralikUzar()
    {
        Kur(Saat(6), Saat(8), KiralamaDurumu.Aktif);

        _zaman.Advance(TimeSpan.FromHours(2)); // şimdi 12:00

        Assert.False(_servis.MusaitMi(_sup.Id, Saat(11), Saat(12)));
    }

    [Fact]
    public void TamamlanmisVeIptalEdilmisKiralamalar_Engellemez()
    {
        Kur(Saat(14), Saat(16), KiralamaDurumu.Tamamlandi);
        Kur(Saat(14), Saat(16), KiralamaDurumu.IptalEdildi);

        Assert.True(_servis.MusaitMi(_sup.Id, Saat(14), Saat(16)));
    }

    [Theory]
    [InlineData(EkipmanDurumu.Bakimda)]
    [InlineData(EkipmanDurumu.HizmetDisi)]
    public void BakimdaVeyaHizmetDisi_HicbirAraliktaMusaitDegil(EkipmanDurumu durum)
    {
        var sup = _db.SupEkle("SUP-002", durum);

        Assert.False(_servis.MusaitMi(sup.Id, Saat(14), Saat(16)));
        Assert.False(_servis.MusaitMi(sup.Id, Bugun.AddDays(30), Bugun.AddDays(31)));
        Assert.DoesNotContain(_servis.MusaitEkipmanlariGetir<Ekipman>(Saat(14), Saat(16)), e => e.Id == sup.Id);
    }

    [Fact]
    public void HaricKiralama_KendisiyleCakismaSayilmaz()
    {
        var rezervasyon = Kur(Saat(14), Saat(16), KiralamaDurumu.Rezerve);

        Assert.True(_servis.MusaitMi(_sup.Id, Saat(13), Saat(16), rezervasyon.Id));
    }

    [Fact]
    public void MusaitEkipmanlariGetir_TureGoreVeCakismayaGoreFiltreler()
    {
        var bosKano = _db.KanoEkle("KANO-001");
        var doluKano = _db.KanoEkle("KANO-002");
        Kur(Saat(14), Saat(16), KiralamaDurumu.Rezerve, doluKano);

        var kanolar = _servis.MusaitEkipmanlariGetir<Kano>(Saat(15), Saat(17));
        var hepsi = _servis.MusaitEkipmanlariGetir<Ekipman>(Saat(15), Saat(17));

        Assert.Equal(bosKano.Id, Assert.Single(kanolar).Id);
        Assert.Equal([_sup.Id, bosKano.Id], hepsi.Select(e => e.Id).Order());
    }

    [Fact]
    public void BitisBaslangictanOnceVeyaEsit_DogrulamaException()
    {
        Assert.Throws<DogrulamaException>(() => _servis.MusaitMi(_sup.Id, Saat(14), Saat(14)));
        Assert.Throws<DogrulamaException>(() => _servis.MusaitEkipmanlariGetir<Ekipman>(Saat(14), Saat(13)));
    }

    [Fact]
    public void OlmayanEkipman_KayitBulunamadi()
    {
        Assert.Throws<KayitBulunamadiException>(() => _servis.MusaitMi(999, Saat(14), Saat(16)));
    }
}
