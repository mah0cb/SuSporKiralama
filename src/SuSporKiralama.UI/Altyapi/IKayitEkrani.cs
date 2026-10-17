namespace SuSporKiralama.UI.Altyapi;

/// <summary>Ana formdaki "Kayıt İşlemleri" Ribbon sayfasındaki butonlar.</summary>
public enum KayitIslemi
{
    Ekle, Duzenle, Sil, Yenile, ExcelAktar,
    FiyatGuncelle, DurumDegistir,       // Ekipman ekranı
    SifreDegistir, AktiflikDegistir     // Personel ekranı
}

/// <summary>
/// Kayıt listesi ekranlarının (Ekipman, Müşteri, Personel) ana formla sözleşmesi. Ana form hangi ekranın
/// açık olduğunu bilmeden butonları bu arayüz üzerinden gösterir/gizler ve tıklamayı ekrana iletir.
/// </summary>
public interface IKayitEkrani
{
    /// <summary>Buton bu ekranda var mı ve oturumdaki personelin yetkisi yetiyor mu?</summary>
    bool IslemGorunur(KayitIslemi islem);

    void Calistir(KayitIslemi islem);
}
