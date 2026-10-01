using Microsoft.Extensions.Time.Testing;
using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Tests;

public class KiralamaServisiTestleri : IDisposable
{
    // "Şimdi" 1 Temmuz 10:00. SUP: 150 TL/saat, 1500 TL depozito; yelek: 50 TL sabit, 200 TL depozito.
    private static readonly DateTime Bugun = new(2026, 7, 1);
    private static DateTime Saat(int saat, int dakika = 0) => Bugun.AddHours(saat).AddMinutes(dakika);

    private readonly TestVeritabani _db = new();
    private readonly FakeTimeProvider _zaman = new(new DateTimeOffset(Saat(10), TimeSpan.Zero));
    private readonly KiralamaServisi _servis;
    private readonly Musteri _musteri;
    private readonly Personel _personel;
    private readonly SupBoard _sup;
    private readonly CanYelegi _yelek;

    public KiralamaServisiTestleri()
    {
        _musteri = _db.MusteriEkle();
        _personel = _db.PersonelEkle();
        _sup = _db.SupEkle();
        _yelek = _db.YelekEkle();

        // Kiralamayı yapan personel oturumdan alınır; testler kayıtlı _personel ile oturum açar.
        _servis = Servis(TestOturumu.PersonelOlarakGiris(_personel));
    }

    public void Dispose() => _db.Dispose();

    private KiralamaServisi Servis(Oturum oturum)
    {
        var musaitlik = new MusaitlikServisi(_db.Repo<Ekipman>(), _db.Repo<KiralamaDetay>(), _zaman);
        return new KiralamaServisi(_db.Repo<Kiralama>(), _db.Repo<Musteri>(), _db.Repo<Personel>(),
            _db.Repo<Ekipman>(), musaitlik, oturum, oturum.Yetki(), _zaman);
    }

    private void SaatiAyarla(DateTime an) => _zaman.SetUtcNow(new DateTimeOffset(an, TimeSpan.Zero));

    private Kiralama Rezervasyon(DateTime baslangic, DateTime bitis, params Ekipman[] ekipmanlar) =>
        _servis.RezervasyonOlustur(_musteri.Id, IdLer(ekipmanlar), baslangic, bitis);

    private Kiralama Kapidan(DateTime bitis, params Ekipman[] ekipmanlar) =>
        _servis.KiralamaBaslat(_musteri.Id, IdLer(ekipmanlar), bitis);

    private IEnumerable<int> IdLer(Ekipman[] ekipmanlar) =>
        (ekipmanlar.Length > 0 ? ekipmanlar : [_sup]).Select(e => e.Id);

    private void DegisiklikKalmamali() => Assert.False(_db.Context.ChangeTracker.HasChanges());

    // --- Akışlar ---

    [Fact]
    public void Rezervasyon_Teslim_Iade_TamAkis()
    {
        var kiralama = Rezervasyon(Saat(14), Saat(16));
        Assert.Equal(KiralamaDurumu.Rezerve, kiralama.Durum);
        Assert.Equal(EkipmanDurumu.Musait, _sup.Durum);
        Assert.Equal(DepozitoDurumu.Alinmadi, kiralama.DepozitoDurumu);

        SaatiAyarla(Saat(13, 50));
        _servis.TeslimEt(kiralama.Id);

        Assert.Equal(KiralamaDurumu.Aktif, kiralama.Durum);
        Assert.Equal(Saat(13, 50), kiralama.BaslangicZamani); // gerçek teslim anı
        Assert.Equal(EkipmanDurumu.Kirada, _sup.Durum);
        Assert.Equal(1500m, kiralama.DepozitoTutari);

        SaatiAyarla(Saat(16));
        _servis.IadeAl(kiralama.Id);

        var kayitli = _servis.IdIleGetir(kiralama.Id);
        Assert.Equal(KiralamaDurumu.Tamamlandi, kayitli.Durum);
        Assert.Equal(Saat(16), kayitli.GercekBitisZamani);
        Assert.Equal(450m, kayitli.ToplamUcret); // 2 saat 10 dk → 3 saat
        Assert.Equal(450m, kayitli.Detaylar.Single().HesaplananUcret);
        Assert.Equal(DepozitoDurumu.IadeEdildi, kayitli.DepozitoDurumu);
        Assert.Equal(EkipmanDurumu.Musait, _sup.Durum);
        DegisiklikKalmamali();
    }

    [Fact]
    public void KapidanKiralama_Iade()
    {
        var kiralama = Kapidan(Saat(12), _sup, _yelek);

        Assert.Equal(KiralamaDurumu.Aktif, kiralama.Durum);
        Assert.Equal(Saat(10), kiralama.BaslangicZamani);
        Assert.Equal(1700m, kiralama.DepozitoTutari);
        Assert.Equal(DepozitoDurumu.Alindi, kiralama.DepozitoDurumu);
        Assert.All(kiralama.Detaylar, d => Assert.Equal(EkipmanDurumu.Kirada, d.Ekipman.Durum));

        _zaman.Advance(TimeSpan.FromMinutes(45));
        _servis.IadeAl(kiralama.Id);

        Assert.Equal(200m, kiralama.ToplamUcret); // SUP 1 saat (150) + yelek sabit (50)
        Assert.All(kiralama.Detaylar, d => Assert.Equal(EkipmanDurumu.Musait, d.Ekipman.Durum));
    }

    [Fact]
    public void IptalEt_Rezervasyon_EkipmanTekrarMusait()
    {
        var kiralama = Rezervasyon(Saat(14), Saat(16));

        _servis.IptalEt(kiralama.Id);

        Assert.Equal(KiralamaDurumu.IptalEdildi, _servis.IdIleGetir(kiralama.Id).Durum);
        Rezervasyon(Saat(14), Saat(16)); // aynı aralık artık boş
    }

    // --- Geçersiz geçişler ---

    [Fact]
    public void IptalEt_AktifKiralama_IslemYapilamaz()
    {
        var kiralama = Kapidan(Saat(12));

        var hata = Assert.Throws<IslemYapilamazException>(() => _servis.IptalEt(kiralama.Id));
        Assert.IsType<InvalidOperationException>(hata.InnerException);
        Assert.Equal(KiralamaDurumu.Aktif, kiralama.Durum);
    }

    [Fact]
    public void IadeAl_RezerveKiralama_IslemYapilamaz()
    {
        var kiralama = Rezervasyon(Saat(14), Saat(16));

        Assert.Throws<IslemYapilamazException>(() => _servis.IadeAl(kiralama.Id));
        Assert.Equal(KiralamaDurumu.Rezerve, kiralama.Durum);
    }

    [Fact]
    public void TeslimEt_TamamlanmisKiralama_IslemYapilamaz()
    {
        var kiralama = Kapidan(Saat(12));
        _servis.IadeAl(kiralama.Id);

        Assert.Throws<IslemYapilamazException>(() => _servis.TeslimEt(kiralama.Id));
    }

    [Fact]
    public void TeslimEt_PlanlananBitisGecmisRezervasyon_IslemYapilamaz()
    {
        var kiralama = Rezervasyon(Saat(14), Saat(16));
        SaatiAyarla(Saat(16, 30));

        Assert.Throws<IslemYapilamazException>(() => _servis.TeslimEt(kiralama.Id));
        Assert.Equal(KiralamaDurumu.Rezerve, kiralama.Durum);
    }

    [Fact]
    public void IadeAl_SaatGeriAlinmis_IslemYapilamazHicbirSeyDegismez()
    {
        var kiralama = Kapidan(Saat(12));
        var detayId = kiralama.Detaylar.Single().Id;

        // Yerel saat 1 saat geri gider (yaz saati bitişi gibi): şimdi teslim anından öncedir.
        _zaman.SetLocalTimeZone(TimeZoneInfo.CreateCustomTimeZone("Eksi1", TimeSpan.FromHours(-1), "Eksi1", "Eksi1"));

        Assert.Throws<IslemYapilamazException>(() =>
            _servis.IadeAl(kiralama.Id, [new HasarBilgisi(detayId, "Çizik", 100m)]));

        Assert.Equal(KiralamaDurumu.Aktif, kiralama.Durum);
        Assert.Empty(kiralama.Detaylar.Single().HasarKayitlari);
        DegisiklikKalmamali();
    }

    [Fact]
    public void OlmayanKiralama_KayitBulunamadi()
    {
        Assert.Throws<KayitBulunamadiException>(() => _servis.TeslimEt(999));
        Assert.Throws<KayitBulunamadiException>(() => _servis.IadeAl(999));
        Assert.Throws<KayitBulunamadiException>(() => _servis.IptalEt(999));
    }

    // --- Oluşturma kuralları ---

    [Fact]
    public void Rezervasyon_GecmisBaslangic_DogrulamaException()
    {
        Assert.Throws<DogrulamaException>(() => Rezervasyon(Saat(9), Saat(11)));
        Assert.Throws<DogrulamaException>(() => Rezervasyon(Saat(10), Saat(11))); // tam şimdi de gelecek değil
    }

    [Fact]
    public void Rezervasyon_MusaitOlmayanEkipman_HicbirSeyKaydedilmez()
    {
        Rezervasyon(Saat(14), Saat(16), _sup);

        var hata = Assert.Throws<IslemYapilamazException>(() => Rezervasyon(Saat(15), Saat(17), _yelek, _sup));

        Assert.Contains(_sup.Kod, hata.Message);
        Assert.DoesNotContain(_yelek.Kod, hata.Message);
        Assert.Single(_db.Context.Kiralamalar);
        DegisiklikKalmamali();
    }

    [Fact]
    public void KiralamaBaslat_BakimdakiEkipman_IslemYapilamaz()
    {
        var bakimda = _db.SupEkle("SUP-002", EkipmanDurumu.Bakimda);

        Assert.Throws<IslemYapilamazException>(() => Kapidan(Saat(12), bakimda));
        DegisiklikKalmamali();
    }

    [Fact]
    public void KiralamaBaslat_GecmisBitis_DogrulamaException()
    {
        Assert.Throws<DogrulamaException>(() => Kapidan(Saat(9)));
    }

    [Fact]
    public void OturumdakiPersonelSonradanPasif_IslemYapilamaz()
    {
        _personel.AktifMi = false;
        _db.Context.SaveChanges();

        Assert.Throws<IslemYapilamazException>(() => Kapidan(Saat(12)));
    }

    [Fact]
    public void PasifMusteri_IslemYapilamaz()
    {
        _musteri.AktifMi = false;
        _db.Context.SaveChanges();

        Assert.Throws<IslemYapilamazException>(() => Kapidan(Saat(12)));
    }

    [Fact]
    public void OlmayanMusteriPersonelVeyaEkipman_KayitBulunamadi()
    {
        // Oturumdaki personel veritabanında yok (kaydedilmemiş, Id = 0).
        var kayitsizPersonelle = Servis(TestOturumu.PersonelOlarakGiris());

        Assert.Throws<KayitBulunamadiException>(() => _servis.KiralamaBaslat(999, [_sup.Id], Saat(12)));
        Assert.Throws<KayitBulunamadiException>(() => kayitsizPersonelle.KiralamaBaslat(_musteri.Id, [_sup.Id], Saat(12)));
        Assert.Throws<KayitBulunamadiException>(() => _servis.KiralamaBaslat(_musteri.Id, [999], Saat(12)));
    }

    [Fact]
    public void BosVeyaTekrarliEkipmanListesi_DogrulamaException()
    {
        Assert.Throws<DogrulamaException>(() => _servis.KiralamaBaslat(_musteri.Id, [], Saat(12)));
        Assert.Throws<DogrulamaException>(() => _servis.KiralamaBaslat(_musteri.Id, [_sup.Id, _sup.Id], Saat(12)));
    }

    // --- Oturum ve yetki ---

    [Fact]
    public void Kiralama_OturumdakiPersonelAdinaKaydedilir()
    {
        var admin = _db.PersonelEkle("admin", Rol.Admin);

        var kiralama = Servis(TestOturumu.AdminOlarakGiris(admin)).KiralamaBaslat(_musteri.Id, [_sup.Id], Saat(12));

        Assert.Equal(admin.Id, _db.Context.Kiralamalar.Single(k => k.Id == kiralama.Id).PersonelId);
    }

    [Fact]
    public void OturumYok_KiralamaVeOdemeYetkisiz()
    {
        var kiralama = TamamlanmisKiralama();
        var oturumsuz = Servis(new Oturum());

        var hata = Assert.Throws<YetkisizIslemException>(() => oturumsuz.KiralamaBaslat(_musteri.Id, [_sup.Id], Saat(13)));
        Assert.Contains("giriş", hata.Message);
        Assert.Throws<YetkisizIslemException>(() => oturumsuz.RezervasyonOlustur(_musteri.Id, [_sup.Id], Saat(14), Saat(15)));
        Assert.Throws<YetkisizIslemException>(() => oturumsuz.OdemeEkle(kiralama.Id, 10m, OdemeTipi.Nakit));

        Assert.Single(_db.Context.Kiralamalar);
        Assert.Empty(_db.Context.Odemeler);
    }

    [Fact]
    public void PersonelRolu_OdemeAlabilir()
    {
        var kiralama = TamamlanmisKiralama();

        _servis.OdemeEkle(kiralama.Id, 350m, OdemeTipi.Nakit);

        Assert.Equal(0m, kiralama.KalanBorc);
    }

    [Fact]
    public void TeslimEt_EkipmanGecikmisBaskaKiralamada_IslemYapilamaz()
    {
        // Rezervasyon yapıldığında SUP 10:30'da dönecekti; teslim anında (10:45) hâlâ dönmedi.
        _db.KiralamaKur(_musteri, _personel, Saat(8), Saat(10, 30), KiralamaDurumu.Aktif, _sup);
        var rezervasyon = Rezervasyon(Saat(11), Saat(13));

        SaatiAyarla(Saat(10, 45));

        Assert.Throws<IslemYapilamazException>(() => _servis.TeslimEt(rezervasyon.Id));
        Assert.Equal(KiralamaDurumu.Rezerve, rezervasyon.Durum);
        DegisiklikKalmamali();
    }

    // --- Hasarlı iade ve depozito ---

    [Fact]
    public void IadeAl_Hasarli_EkipmanBakimdaDigerleriMusait()
    {
        var kiralama = Kapidan(Saat(12), _sup, _yelek);
        var supDetay = kiralama.Detaylar.Single(d => d.EkipmanId == _sup.Id);
        _zaman.Advance(TimeSpan.FromHours(1));

        _servis.IadeAl(kiralama.Id, [new HasarBilgisi(supDetay.Id, "Kanat kırık", 300m)]);

        var kayitli = _servis.IdIleGetir(kiralama.Id);
        Assert.Equal(EkipmanDurumu.Bakimda, _sup.Durum);
        Assert.Equal(EkipmanDurumu.Musait, _yelek.Durum);
        Assert.Equal(500m, kayitli.ToplamUcret); // 150 + 50 + 300 hasar
        var hasar = Assert.Single(kayitli.Detaylar.Single(d => d.Id == supDetay.Id).HasarKayitlari);
        Assert.Equal(Saat(11), hasar.KayitTarihi);
        Assert.Equal(300m, kayitli.DepozitoMahsupTutari);
        Assert.Equal(DepozitoDurumu.KismenIadeEdildi, kayitli.DepozitoDurumu);
        Assert.Equal(200m, kayitli.KalanBorc); // hasar depozitodan düşüldü, kiralama ücreti kaldı
    }

    [Theory]
    [InlineData(1000, DepozitoDurumu.KismenIadeEdildi, 1000, 150)] // hasar < depozito
    [InlineData(1500, DepozitoDurumu.MahsupEdildi, 1500, 150)]     // hasar = depozito
    [InlineData(2000, DepozitoDurumu.MahsupEdildi, 1500, 650)]     // hasar > depozito: aşan 500 borca eklenir
    public void IadeAl_DepozitoMahsubu(decimal hasarBedeli, DepozitoDurumu durum, decimal mahsup, decimal kalanBorc)
    {
        var kiralama = Kapidan(Saat(11));

        _servis.IadeAl(kiralama.Id, [new HasarBilgisi(kiralama.Detaylar.Single().Id, "Delik", hasarBedeli)]);

        Assert.Equal(durum, kiralama.DepozitoDurumu);
        Assert.Equal(mahsup, kiralama.DepozitoMahsupTutari);
        Assert.Equal(kalanBorc, kiralama.KalanBorc);
    }

    [Fact]
    public void IadeAl_BaskaKiralamaninSatiri_HicbirSeyDegismez()
    {
        var baska = Kapidan(Saat(12), _yelek);
        var kiralama = Kapidan(Saat(12), _sup);

        Assert.Throws<DogrulamaException>(() =>
            _servis.IadeAl(kiralama.Id, [new HasarBilgisi(baska.Detaylar.Single().Id, "Çizik", 100m)]));

        Assert.Equal(KiralamaDurumu.Aktif, kiralama.Durum);
        Assert.Empty(_db.Context.HasarKayitlari);
        DegisiklikKalmamali();
    }

    [Fact]
    public void IadeAl_NegatifHasarBedeli_HicbirSeyDegismez()
    {
        var kiralama = Kapidan(Saat(12));
        var detayId = kiralama.Detaylar.Single().Id;

        Assert.Throws<DogrulamaException>(() =>
            _servis.IadeAl(kiralama.Id, [new HasarBilgisi(detayId, "Çizik", 100m), new HasarBilgisi(detayId, "Delik", -1m)]));

        Assert.Equal(KiralamaDurumu.Aktif, kiralama.Durum);
        Assert.Empty(kiralama.Detaylar.Single().HasarKayitlari);
        DegisiklikKalmamali();
    }

    // --- Ödeme ---

    private Kiralama TamamlanmisKiralama()
    {
        var kiralama = Kapidan(Saat(12), _sup, _yelek);
        _zaman.Advance(TimeSpan.FromHours(2));
        _servis.IadeAl(kiralama.Id); // 2 saat SUP (300) + yelek (50) = 350
        return kiralama;
    }

    [Fact]
    public void OdemeEkle_KismiVeKalanOdeme_BorcKapanir()
    {
        var kiralama = TamamlanmisKiralama();

        _servis.OdemeEkle(kiralama.Id, 100m, OdemeTipi.Nakit, "Nakit kısmı");
        Assert.Equal(250m, kiralama.KalanBorc);

        var odeme = _servis.OdemeEkle(kiralama.Id, 250m, OdemeTipi.KrediKarti);

        Assert.Equal(Saat(12), odeme.OdemeTarihi);
        Assert.Equal(0m, _servis.IdIleGetir(kiralama.Id).KalanBorc);
        Assert.Equal(2, _db.Context.Odemeler.Count());
    }

    [Fact]
    public void OdemeEkle_FazlaOdeme_IslemYapilamaz()
    {
        var kiralama = TamamlanmisKiralama();
        _servis.OdemeEkle(kiralama.Id, 300m, OdemeTipi.Nakit);

        Assert.Throws<IslemYapilamazException>(() => _servis.OdemeEkle(kiralama.Id, 50.01m, OdemeTipi.Nakit));

        Assert.Equal(50m, kiralama.KalanBorc);
        DegisiklikKalmamali();
    }

    [Fact]
    public void OdemeEkle_HasarDepozitoyuAsinca_AsanKisimOdenebilir()
    {
        var kiralama = Kapidan(Saat(11));
        _servis.IadeAl(kiralama.Id, [new HasarBilgisi(kiralama.Detaylar.Single().Id, "Delik", 2000m)]);

        // 150 kiralama + (2000 - 1500 depozito) = 650
        _servis.OdemeEkle(kiralama.Id, 650m, OdemeTipi.Havale);

        Assert.Equal(0m, kiralama.KalanBorc);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void OdemeEkle_SifirVeyaNegatifTutar_DogrulamaException(decimal tutar)
    {
        var kiralama = TamamlanmisKiralama();

        Assert.Throws<DogrulamaException>(() => _servis.OdemeEkle(kiralama.Id, tutar, OdemeTipi.Nakit));
        DegisiklikKalmamali();
    }

    [Fact]
    public void OdemeEkle_TamamlanmamisKiralama_IslemYapilamaz()
    {
        var kiralama = Kapidan(Saat(12));

        Assert.Throws<IslemYapilamazException>(() => _servis.OdemeEkle(kiralama.Id, 100m, OdemeTipi.Nakit));
    }

    // --- Gecikme ve sorgular ---

    [Fact]
    public void Gecikme_SorgudaGorunurUcretGercekSuredenHesaplanir()
    {
        var kiralama = Kapidan(Saat(11));
        SaatiAyarla(Saat(12, 30));

        Assert.Equal(kiralama.Id, Assert.Single(_servis.GecikmisKiralamalar()).Id);
        Assert.Equal(TimeSpan.FromMinutes(90), kiralama.GecikmeSuresi(Saat(12, 30)));

        _servis.IadeAl(kiralama.Id);

        Assert.Empty(_servis.GecikmisKiralamalar());
        Assert.Equal(450m, kiralama.ToplamUcret); // 2,5 saat → 3 saat; ayrıca ceza yok
        Assert.Equal(TimeSpan.FromMinutes(90), kiralama.GecikmeSuresi(Saat(20)));
    }

    [Fact]
    public void AktifKiralamalar_SadeceAktifler()
    {
        var aktif = Kapidan(Saat(12), _sup);
        Rezervasyon(Saat(14), Saat(16), _yelek);

        Assert.Equal(aktif.Id, Assert.Single(_servis.AktifKiralamalar()).Id);
    }

    [Fact]
    public void BugunkuRezervasyonlar_SadeceBugunBaslayanRezerveler()
    {
        var bugunku = Rezervasyon(Saat(14), Saat(16));
        Rezervasyon(Bugun.AddDays(1).AddHours(10), Bugun.AddDays(1).AddHours(12));
        var iptal = Rezervasyon(Saat(17), Saat(18));
        _servis.IptalEt(iptal.Id);

        Assert.Equal(bugunku.Id, Assert.Single(_servis.BugunkuRezervasyonlar()).Id);
    }

    [Fact]
    public void MusteriGecmisi_SadeceOMusteriEnYenidenEskiye()
    {
        var eski = _db.KiralamaKur(_musteri, _personel, Saat(-48), Saat(-46), KiralamaDurumu.Tamamlandi, _sup);
        var yeni = Rezervasyon(Saat(14), Saat(16));
        _db.KiralamaKur(_db.MusteriEkle("05329999999"), _personel, Saat(-24), Saat(-22), KiralamaDurumu.Tamamlandi, _sup);

        var gecmis = _servis.MusteriGecmisi(_musteri.Id);

        Assert.Equal([yeni.Id, eski.Id], gecmis.Select(k => k.Id));
    }
}
