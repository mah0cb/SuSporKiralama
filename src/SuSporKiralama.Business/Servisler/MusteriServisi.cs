using System.Globalization;
using System.Text.RegularExpressions;
using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.Business.Servisler.Soyut;
using SuSporKiralama.DataAccess.Repository;
using SuSporKiralama.Entities;

namespace SuSporKiralama.Business.Servisler;

public class MusteriServisi(IRepository<Musteri> repository, IRepository<Kiralama> kiralamaRepository, IYetkiServisi yetki)
    : CrudServisiTemel<Musteri>(repository, yetki), IMusteriServisi
{
    // Türkiye cep telefonu, normalize edilmiş hali: 05XXXXXXXXX (11 hane).
    private static readonly Regex TelefonBicimi = new("^05[0-9]{9}$");

    // Entity'deki MailAddress kontrolü "ali@x" gibi adresleri de kabul eder;
    // burada alan adında nokta olması da istenir.
    private static readonly Regex EpostaBicimi = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    private static readonly CompareInfo TurkceKarsilastirma = CultureInfo.GetCultureInfo("tr-TR").CompareInfo;

    protected override string EntityAdi => "Müşteri";
    protected override Islem YonetimIslemi => Islem.MusteriIslemleri;

    // Ekleme ve güncelleme aynı kuralları kullanır (GuncellemeOncesiKontrol varsayılan olarak bunu çağırır).
    protected override void EklemeOncesiKontrol(Musteri entity)
    {
        entity.Telefon = TelefonNormalizeEt(entity.Telefon);

        if (entity.Eposta is not null && !EpostaBicimi.IsMatch(entity.Eposta))
            throw new DogrulamaException("Geçerli bir e-posta adresi giriniz.");

        // Id != entity.Id: güncellemede kaydın kendi telefonu çakışma sayılmaz (eklemede Id = 0).
        var telefon = entity.Telefon;
        if (Repository.Find(m => m.Telefon == telefon && m.Id != entity.Id).Count > 0)
            throw new BenzersizlikIhlaliException("Telefon", telefon);
    }

    protected override void SilmeOncesiKontrol(Musteri entity)
    {
        if (kiralamaRepository.Find(k => k.MusteriId == entity.Id).Count > 0)
            throw new IliskiliKayitVarException(
                $"{entity.AdSoyad} adlı müşterinin kiralama geçmişi olduğu için silinemez. Bunun yerine müşteriyi pasif yapabilirsiniz.");
    }

    public List<Musteri> Ara(string metin)
    {
        if (string.IsNullOrWhiteSpace(metin))
            return TumunuGetir();

        var aranan = metin.Trim();
        var arananRakamlar = new string(aranan.Where(char.IsAsciiDigit).ToArray());

        // Karşılaştırma bellekte ve tr-TR kurallarıyla yapılır ("YILMAZ" = "Yılmaz");
        // veritabanının harf duyarlılığı sağlayıcıya göre değiştiği için sorguya bırakılmadı.
        // Müşteri sayısı binlerle ifade edilirse arama sorguya taşınmalıdır.
        return TumunuGetir()
            .Where(m => TurkceKarsilastirma.IndexOf(m.AdSoyad, aranan, CompareOptions.IgnoreCase) >= 0
                        || (arananRakamlar.Length > 0 && m.Telefon.Contains(arananRakamlar)))
            .ToList();
    }

    /// <summary>
    /// "0532 111 22 33", "+90 532 111 22 33", "5321112233" gibi yazımları 05321112233 biçimine
    /// çevirir. Böylece aynı numara farklı yazılarak ikinci kez kaydedilemez.
    /// </summary>
    private static string TelefonNormalizeEt(string telefon)
    {
        var sade = new string(telefon.Where(c => c is not (' ' or '-' or '(' or ')')).ToArray());

        if (sade.StartsWith("+90"))
            sade = "0" + sade[3..];
        else if (sade.StartsWith("90") && sade.Length == 12)
            sade = "0" + sade[2..];
        else if (sade.StartsWith('5') && sade.Length == 10)
            sade = "0" + sade;

        if (!TelefonBicimi.IsMatch(sade))
            throw new DogrulamaException("Telefon geçerli bir cep telefonu numarası olmalıdır (ör. 0532 123 45 67).");
        return sade;
    }
}
