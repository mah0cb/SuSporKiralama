using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Business.Servisler;

/// <summary>
/// Ekipmanın belirli bir zaman aralığında kiralanabilir olup olmadığını hesaplar.
/// Aralıklar yarı açıktır: [baslangic, bitis). 14:00'te biten ve 14:00'te başlayan
/// iki kiralama çakışmaz.
/// </summary>
public interface IMusaitlikServisi
{
    /// <summary>
    /// Ekipman Bakımda/Hizmet Dışı değilse ve aralık Rezerve/Aktif başka bir kiralamayla
    /// çakışmıyorsa true. haricKiralamaId: kontrolde yok sayılacak kiralama (ör. teslim edilen rezervasyonun kendisi).
    /// </summary>
    bool MusaitMi(int ekipmanId, DateTime baslangic, DateTime bitis, int? haricKiralamaId = null);

    /// <summary>
    /// Aralıkta müsait ekipmanlar, türe göre: MusaitEkipmanlariGetir&lt;Kano&gt;(...) sadece kanolar,
    /// MusaitEkipmanlariGetir&lt;Ekipman&gt;(...) hepsi.
    /// </summary>
    List<TEkipman> MusaitEkipmanlariGetir<TEkipman>(DateTime baslangic, DateTime bitis) where TEkipman : Ekipman;
}
