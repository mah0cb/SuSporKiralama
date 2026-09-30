using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Business.Servisler.Soyut;
using SuSporKiralama.DataAccess.Repository;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.Business.Servisler;

public class EkipmanServisi(IRepository<Ekipman> repository, IRepository<KiralamaDetay> kiralamaDetayRepository)
    : CrudServisiTemel<Ekipman>(repository), IEkipmanServisi
{
    protected override string EntityAdi => "Ekipman";

    // Ekleme ve güncelleme aynı kuralları kullanır (GuncellemeOncesiKontrol varsayılan olarak bunu çağırır).
    protected override void EklemeOncesiKontrol(Ekipman entity)
    {
        if (entity.BirimUcret <= 0)
            throw new DogrulamaException("Birim ücret sıfırdan büyük olmalıdır.");

        var id = entity.Id;
        var kod = entity.Kod;
        if (Repository.Find(e => e.Kod == kod && e.Id != id).Count > 0)
            throw new BenzersizlikIhlaliException("Ekipman kodu", kod);

        // Find filtresi veritabanında çalışır, yani burada bellekteki değil kayıtlı durum okunur.
        // Eklemede (Id = 0) kayıt olmadığı için "kayıtlı durum Kirada değil" sayılır.
        var kayitliDurumKirada = Repository.Find(e => e.Id == id && e.Durum == EkipmanDurumu.Kirada).Count > 0;
        var yeniDurumKirada = entity.Durum == EkipmanDurumu.Kirada;

        if (yeniDurumKirada && !kayitliDurumKirada)
            throw new IslemYapilamazException(
                "Ekipman elle Kirada durumuna alınamaz; bu durum kiralama başlatılınca otomatik verilir.");
        if (kayitliDurumKirada && !yeniDurumKirada)
            throw new IslemYapilamazException(
                "Kiradaki ekipmanın durumu elle değiştirilemez; ekipman iade alındığında Müsait olur.");
    }

    protected override void SilmeOncesiKontrol(Ekipman entity)
    {
        if (kiralamaDetayRepository.Find(d => d.EkipmanId == entity.Id).Count > 0)
            throw new IliskiliKayitVarException(
                $"{entity.Kod} kodlu ekipmanın kiralama geçmişi olduğu için silinemez. Bunun yerine durumunu Hizmet Dışı yapın.");
    }

    // "e is TEkipman" EF Core tarafından discriminator (EkipmanTipi) sütunu üzerinden sorguya çevrilir.
    public List<TEkipman> Filtrele<TEkipman>(EkipmanDurumu? durum = null) where TEkipman : Ekipman =>
        Repository.Find(e => e is TEkipman && (durum == null || e.Durum == durum))
            .Cast<TEkipman>()
            .ToList();

    public List<Ekipman> MusaitleriGetir() => Filtrele<Ekipman>(EkipmanDurumu.Musait);

    public void FiyatGuncelle(int id, decimal yeniBirimUcret)
    {
        var ekipman = IdIleGetir(id);
        DogrulamaIle(() => ekipman.BirimUcret = yeniBirimUcret);
        Guncelle(ekipman); // sıfır fiyat burada reddedilir ve değişiklik geri alınır
    }

    public void DurumDegistir(int id, EkipmanDurumu yeniDurum)
    {
        var ekipman = IdIleGetir(id);
        ekipman.Durum = yeniDurum;
        Guncelle(ekipman); // Kirada kuralı EklemeOncesiKontrol'de
    }
}
