using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.DataAccess.Konfigurasyonlar;

/// <summary>
/// Ekipman hiyerarşisi TPH (Table-Per-Hierarchy) ile eşlenir: SupBoard, Kano ve CanYelegi
/// tek bir "Ekipmanlar" tablosunda tutulur. Hangi satırın hangi tür olduğunu
/// "EkipmanTipi" sütunu (discriminator) belirtir. Alt türe özgü sütunlar
/// (UzunlukCm, KisiKapasitesi, Beden...) diğer türlerde NULL kalır.
/// </summary>
public class EkipmanConfiguration : IEntityTypeConfiguration<Ekipman>
{
    public void Configure(EntityTypeBuilder<Ekipman> builder)
    {
        builder.ToTable("Ekipmanlar");

        builder.HasDiscriminator<string>("EkipmanTipi")
            .HasValue<SupBoard>(nameof(SupBoard))
            .HasValue<Kano>(nameof(Kano))
            .HasValue<CanYelegi>(nameof(CanYelegi));
        builder.Property("EkipmanTipi").HasMaxLength(20);

        builder.Property(e => e.Kod).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Marka).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Model).IsRequired().HasMaxLength(50);

        // Ekipman kodu (ör. SUP-001) benzersizdir.
        builder.HasIndex(e => e.Kod).IsUnique();
    }
}
