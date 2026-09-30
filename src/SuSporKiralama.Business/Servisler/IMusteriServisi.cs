using SuSporKiralama.Business.Servisler.Soyut;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Business.Servisler;

public interface IMusteriServisi : ICrudServisi<Musteri>
{
    /// <summary>Ad, soyad veya telefonda geçen metne göre arar (büyük/küçük harf duyarsız).</summary>
    List<Musteri> Ara(string metin);
}
