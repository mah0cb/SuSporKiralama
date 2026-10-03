using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Business.Oneri;

// Öneri akışında taşınan veriler: değiştirilemez record'lar.

/// <summary>
/// Personelin girdiği istek. Tarihler arayüzdeki tarih alanlarından gelir, metinden çıkarılmaz.
/// MusteriId verilirse müşterinin deneyim seviyesi öneride dikkate alınır.
/// </summary>
public record OneriIstegi(string MusteriMetni, DateTime Baslangic, DateTime Bitis, int? MusteriId = null);

/// <summary>Sağlayıcıya verilen girdi: yalnızca o aralıkta müsait ekipmanlar.</summary>
public record OneriGirdisi(
    string MusteriMetni,
    DeneyimSeviyesi? Deneyim,
    TimeSpan Sure,
    IReadOnlyList<EkipmanOzeti> MusaitEkipmanlar);

/// <summary>
/// Sağlayıcının görmesi gereken ekipman bilgisi. Türe özgü alanlar diğer türlerde null kalır
/// (SUP: uzunluk, max taşıma, tip; kano: kişi kapasitesi; yelek: beden).
/// </summary>
public record EkipmanOzeti(
    string Kod,
    string Tur,
    decimal BirimUcret,
    string UcretBirimi,
    int? UzunlukCm = null,
    int? MaxTasimaKg = null,
    SupBoardTipi? SupTipi = null,
    int? KisiKapasitesi = null,
    Beden? Beden = null)
{
    /// <summary>Ekipmanın alt türüne göre özet oluşturur (desen eşleme ile).</summary>
    public static EkipmanOzeti Olustur(Ekipman ekipman) => ekipman switch
    {
        SupBoard s => new(s.Kod, nameof(SupBoard), s.BirimUcret, "saatlik",
            UzunlukCm: s.UzunlukCm, MaxTasimaKg: s.MaxTasimaKg, SupTipi: s.Tip),
        Kano k => new(k.Kod, nameof(Kano), k.BirimUcret, "4 saatlik blok", KisiKapasitesi: k.KisiKapasitesi),
        CanYelegi y => new(y.Kod, nameof(CanYelegi), y.BirimUcret, "kiralama başı sabit", Beden: y.Beden),
        _ => throw new ArgumentOutOfRangeException(nameof(ekipman), "Bilinmeyen ekipman türü.")
    };
}

/// <summary>
/// Sağlayıcının ürettiği, henüz doğrulanmamış öneri. Saglayici: öneriyi üretenin adı;
/// Uyari: ör. yapay zeka kullanılamadığı için yedek sağlayıcıya geçildiği bilgisi.
/// </summary>
public record HamOneri(
    IReadOnlyList<string> EkipmanKodlari,
    int KisiSayisi,
    string Aciklama,
    string GuvenlikNotu,
    string Saglayici,
    string? Uyari = null);

/// <summary>Id, öneri arayüzde onaylanınca kiralamaya aktarılabilsin diye taşınır.</summary>
public record OnerilenEkipman(int Id, string Kod, string Tur, decimal TahminiUcret, decimal Depozito);

/// <summary>
/// Doğrulanmış öneri. Ücret ve depozito sağlayıcıdan değil, UcretHesapla ile hesaplanır.
/// EkipmanYeterli false ise Duzeltmeler hangi ekipmanın eksik kaldığını söyler.
/// </summary>
public record OneriSonucu(
    IReadOnlyList<OnerilenEkipman> Ekipmanlar,
    int KisiSayisi,
    string Aciklama,
    string GuvenlikNotu,
    decimal TahminiToplamUcret,
    decimal ToplamDepozito,
    string Saglayici,
    string? SaglayiciUyarisi,
    bool EkipmanYeterli,
    IReadOnlyList<string> Duzeltmeler);
