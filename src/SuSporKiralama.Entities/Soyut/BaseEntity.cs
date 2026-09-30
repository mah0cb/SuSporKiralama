namespace SuSporKiralama.Entities.Soyut;

/// <summary>
/// Tüm entity'lerin ortak atası. Soyut (abstract) olduğu için tek başına nesnesi oluşturulamaz;
/// sadece kalıtım yoluyla kullanılır. Generic repository'deki "where T : BaseEntity" kısıtı
/// bu sınıf sayesinde mümkündür.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    // Kaydın oluşturulma zamanı; nesne oluşturulurken otomatik atanır.
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
}
