using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Business.Servisler.Soyut;
using SuSporKiralama.DataAccess.Guvenlik;
using SuSporKiralama.DataAccess.Repository;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Business.Servisler;

public class PersonelServisi(IRepository<Personel> repository, IRepository<Kiralama> kiralamaRepository)
    : CrudServisiTemel<Personel>(repository), IPersonelServisi
{
    public const int MinSifreUzunlugu = 8;

    protected override string EntityAdi => "Personel";

    public Personel Ekle(string adSoyad, string kullaniciAdi, string sifre, Rol rol)
    {
        SifreKontrol(sifre);

        Personel personel = null!;
        DogrulamaIle(() => personel = new Personel(adSoyad, kullaniciAdi, SifreHasher.Hashle(sifre), rol));

        Ekle(personel); // temel sınıftaki ortak akış (kontrol → kaydet)
        return personel;
    }

    public void SifreDegistir(int id, string yeniSifre)
    {
        SifreKontrol(yeniSifre);
        var personel = IdIleGetir(id);

        // Genel Guncelle şifre değişikliğini reddettiği için burada doğrudan kaydedilir.
        // Önce Reload: nesnede kontrol edilmemiş başka değişiklik varsa şifreyle birlikte yazılmasın.
        Repository.Reload(personel);
        personel.SifreHash = SifreHasher.Hashle(yeniSifre);
        Repository.Update(personel);
        Repository.SaveChanges();
    }

    public void AktiflikDegistir(int id, bool aktif)
    {
        var personel = IdIleGetir(id);
        personel.AktifMi = aktif;
        Guncelle(personel); // son aktif admin kuralı GuncellemeOncesiKontrol'de
    }

    protected override void EklemeOncesiKontrol(Personel entity)
    {
        // Genel Ekle(Personel) yolunda düz şifre verilirse veritabanına düz metin yazılmasın.
        if (!SifreHasher.GecerliHashMi(entity.SifreHash))
            throw new DogrulamaException("Şifre hash'lenmemiş; Ekle(adSoyad, kullaniciAdi, sifre, rol) metodunu kullanın.");

        var id = entity.Id;
        var kullaniciAdi = entity.KullaniciAdi;
        if (Repository.Find(p => p.KullaniciAdi == kullaniciAdi && p.Id != id).Count > 0)
            throw new BenzersizlikIhlaliException("Kullanıcı adı", kullaniciAdi);
    }

    // Override + base çağrısı: eklemedeki ortak kurallar korunur, güncellemeye özel kurallar eklenir.
    protected override void GuncellemeOncesiKontrol(Personel entity)
    {
        base.GuncellemeOncesiKontrol(entity);

        // Find filtresi veritabanında çalışır: hash kayıtlı olanla aynı değilse değiştirilmiş demektir.
        var id = entity.Id;
        var hash = entity.SifreHash;
        if (Repository.Find(p => p.Id == id && p.SifreHash == hash).Count == 0)
            throw new DogrulamaException("Şifre yalnızca şifre değiştirme işlemiyle değiştirilebilir.");

        SonAktifAdminKorunur(entity, islemSonrasiAktifAdmin: entity.Rol == Rol.Admin && entity.AktifMi,
            "Son aktif yönetici pasifleştirilemez ve rolü değiştirilemez; sistemde en az bir aktif yönetici kalmalıdır.");
    }

    protected override void SilmeOncesiKontrol(Personel entity)
    {
        SonAktifAdminKorunur(entity, islemSonrasiAktifAdmin: false,
            "Son aktif yönetici silinemez; sistemde en az bir aktif yönetici kalmalıdır.");

        if (kiralamaRepository.Find(k => k.PersonelId == entity.Id).Count > 0)
            throw new IliskiliKayitVarException(
                $"{entity.AdSoyad} adlı personelin kiralama kayıtları olduğu için silinemez. Bunun yerine personeli pasif yapın.");
    }

    /// <summary>
    /// Kayıt şu an (veritabanında) aktif bir Admin ise ve işlemden sonra öyle kalmayacaksa,
    /// başka en az bir aktif Admin olmalıdır.
    /// </summary>
    private void SonAktifAdminKorunur(Personel personel, bool islemSonrasiAktifAdmin, string mesaj)
    {
        if (islemSonrasiAktifAdmin)
            return;

        var id = personel.Id;
        var kayitliAktifAdmin = Repository.Find(p => p.Id == id && p.Rol == Rol.Admin && p.AktifMi).Count > 0;
        if (!kayitliAktifAdmin)
            return;

        var baskaAktifAdminVar = Repository.Find(p => p.Id != id && p.Rol == Rol.Admin && p.AktifMi).Count > 0;
        if (!baskaAktifAdminVar)
            throw new IslemYapilamazException(mesaj);
    }

    private static void SifreKontrol(string sifre)
    {
        if (string.IsNullOrEmpty(sifre) || sifre.Length < MinSifreUzunlugu)
            throw new DogrulamaException($"Şifre en az {MinSifreUzunlugu} karakter olmalıdır.");
    }
}
