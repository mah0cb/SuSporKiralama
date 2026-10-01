namespace SuSporKiralama.Business.Guvenlik;

/// <summary>Yetki gerektiren işlemler. Hangi rolün hangisini yapabileceği YetkiServisi'ndeki matriste.</summary>
public enum Islem
{
    PersonelYonetimi,       // personel ekle, güncelle, sil, şifre, aktiflik
    MusteriIslemleri,       // müşteri ekle, güncelle, sil
    EkipmanYonetimi,        // ekipman ekle, sil ve genel güncelleme (fiyatı da değiştirebildiği için)
    FiyatGuncelleme,
    EkipmanDurumDegistirme,
    KiralamaIslemleri,      // rezervasyon, başlatma, teslim, iade, iptal
    OdemeAlma,
    RaporGoruntuleme        // dashboard dahil
}
