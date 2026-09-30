using SuSporKiralama.Entities;

namespace SuSporKiralama.Business.Servisler;

/// <summary>
/// Kiralama yalnızca bu iş akışlarıyla değişir; genel Ekle/Guncelle/Sil yoktur.
/// Her işlem tek SaveChanges ile kaydedilir; kural hatasında hiçbir değişiklik kalmaz.
/// </summary>
public interface IKiralamaServisi
{
    /// <summary>Müşteri, detaylar (ekipman ve hasarlarıyla) ve ödemelerle birlikte getirir.</summary>
    Kiralama IdIleGetir(int id);

    /// <summary>İleri tarihli rezervasyon (Rezerve). Başlangıç gelecekte olmalı, ekipmanlar müsait olmalı.</summary>
    Kiralama RezervasyonOlustur(int musteriId, int personelId, IEnumerable<int> ekipmanIdleri,
        DateTime baslangic, DateTime planlananBitis);

    /// <summary>Kapıdan gelen müşteri: başlangıç şimdi, kiralama doğrudan Aktif olur.</summary>
    Kiralama KiralamaBaslat(int musteriId, int personelId, IEnumerable<int> ekipmanIdleri, DateTime planlananBitis);

    /// <summary>Rezervasyonu Aktif yapar; başlangıç gerçek teslim anı olur, müsaitlik yeniden kontrol edilir.</summary>
    void TeslimEt(int kiralamaId);

    /// <summary>İade: ücret gerçek süreden hesaplanır, hasar bedeli depozitodan mahsup edilir.</summary>
    void IadeAl(int kiralamaId, IEnumerable<HasarBilgisi>? hasarlar = null);

    /// <summary>Sadece Rezerve kiralamalar iptal edilebilir.</summary>
    void IptalEt(int kiralamaId);

    /// <summary>
    /// Tamamlanmış kiralamaya ödeme ekler. Tutar pozitif olmalı ve kalan borcu (Kiralama.KalanBorc) aşamaz.
    /// </summary>
    Odeme OdemeEkle(int kiralamaId, decimal tutar, OdemeTipi odemeTipi, string? aciklama = null);

    List<Kiralama> AktifKiralamalar();

    /// <summary>Aktif olup planlanan bitişi geçmiş kiralamalar.</summary>
    List<Kiralama> GecikmisKiralamalar();

    /// <summary>Müşterinin tüm kiralamaları, en yeniden eskiye.</summary>
    List<Kiralama> MusteriGecmisi(int musteriId);

    List<Kiralama> BugunkuRezervasyonlar();
}
