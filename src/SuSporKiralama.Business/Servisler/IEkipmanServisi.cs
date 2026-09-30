using SuSporKiralama.Business.Servisler.Soyut;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Business.Servisler;

public interface IEkipmanServisi : ICrudServisi<Ekipman>
{
    /// <summary>
    /// Türe ve isteğe bağlı olarak duruma göre listeler. Ör. Filtrele&lt;SupBoard&gt;() tüm SUP'lar,
    /// Filtrele&lt;Ekipman&gt;(EkipmanDurumu.Bakimda) bakımdaki tüm ekipmanlar.
    /// </summary>
    List<TEkipman> Filtrele<TEkipman>(EkipmanDurumu? durum = null) where TEkipman : Ekipman;

    /// <summary>Şu an Musait durumdaki ekipmanlar. Zaman aralığına göre müsaitlik için IMusaitlikServisi kullanılır.</summary>
    List<Ekipman> MusaitleriGetir();

    void FiyatGuncelle(int id, decimal yeniBirimUcret);

    /// <summary>Kirada durumuna geçiş ve Kirada'dan çıkış yalnızca kiralama/iade akışıyla olur.</summary>
    void DurumDegistir(int id, EkipmanDurumu yeniDurum);
}
