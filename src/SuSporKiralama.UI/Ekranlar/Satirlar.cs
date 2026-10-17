using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;
using SuSporKiralama.UI.Altyapi;

namespace SuSporKiralama.UI.Ekranlar;

// Liste ekranlarındaki grid'lere entity'nin kendisi değil, bu salt okunur satırlar bağlanır:
// grid yalnızca gösterilecek alanları görür (ör. personelin şifre hash'i hiç yer almaz),
// enum'lar ve türe özel bilgiler okunabilir metin olarak gelir. Id gizli sütundur; işlemler Id ile yapılır.

// DurumDegeri grid'de sütun olarak yok; Durum Değiştir penceresinde mevcut durumu seçili getirmek için.
public sealed record EkipmanSatiri(int Id, string Kod, string Tur, string Marka, string Model, string Durum,
    decimal BirimUcret, decimal Depozito, string Ozellikler, EkipmanDurumu DurumDegeri)
{
    public static EkipmanSatiri Olustur(Ekipman e) => new(e.Id, e.Kod, Metinler.TurAdi(e), e.Marka, e.Model,
        Metinler.Ad(e.Durum), e.BirimUcret, e.DepozitoTutari, Metinler.Ozellikler(e), e.Durum);
}

public sealed record MusteriSatiri(int Id, string AdSoyad, string Telefon, string? Eposta, string Deneyim,
    bool Aktif, DateTime KayitTarihi)
{
    public static MusteriSatiri Olustur(Musteri m) => new(m.Id, m.AdSoyad, m.Telefon, m.Eposta,
        Metinler.Ad(m.DeneyimSeviyesi), m.AktifMi, m.OlusturmaTarihi);
}

public sealed record PersonelSatiri(int Id, string AdSoyad, string KullaniciAdi, string Rol, bool Aktif)
{
    public static PersonelSatiri Olustur(Personel p) => new(p.Id, p.AdSoyad, p.KullaniciAdi, Metinler.Ad(p.Rol), p.AktifMi);
}
