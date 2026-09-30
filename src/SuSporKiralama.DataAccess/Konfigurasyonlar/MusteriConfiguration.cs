using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuSporKiralama.Entities;

namespace SuSporKiralama.DataAccess.Konfigurasyonlar;

public class MusteriConfiguration : IEntityTypeConfiguration<Musteri>
{
    public void Configure(EntityTypeBuilder<Musteri> builder)
    {
        builder.ToTable("Musteriler");

        builder.Property(m => m.Ad).IsRequired().HasMaxLength(50);
        builder.Property(m => m.Soyad).IsRequired().HasMaxLength(50);
        builder.Property(m => m.Telefon).IsRequired().HasMaxLength(20);
        builder.Property(m => m.Eposta).HasMaxLength(100);
        builder.Property(m => m.Notlar).HasMaxLength(500);

        // Müşteri telefonla aranır; aynı numara iki kez kaydedilemez.
        builder.HasIndex(m => m.Telefon).IsUnique();
    }
}
