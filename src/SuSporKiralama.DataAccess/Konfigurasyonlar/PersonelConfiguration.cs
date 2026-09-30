using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuSporKiralama.Entities;

namespace SuSporKiralama.DataAccess.Konfigurasyonlar;

public class PersonelConfiguration : IEntityTypeConfiguration<Personel>
{
    public void Configure(EntityTypeBuilder<Personel> builder)
    {
        builder.ToTable("Personeller");

        builder.Property(p => p.AdSoyad).IsRequired().HasMaxLength(100);
        builder.Property(p => p.KullaniciAdi).IsRequired().HasMaxLength(50);
        builder.Property(p => p.SifreHash).IsRequired().HasMaxLength(200);

        // Aynı kullanıcı adıyla iki personel olamaz.
        builder.HasIndex(p => p.KullaniciAdi).IsUnique();
    }
}
