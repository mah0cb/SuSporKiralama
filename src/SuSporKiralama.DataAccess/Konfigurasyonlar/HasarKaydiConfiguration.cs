using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuSporKiralama.Entities;

namespace SuSporKiralama.DataAccess.Konfigurasyonlar;

public class HasarKaydiConfiguration : IEntityTypeConfiguration<HasarKaydi>
{
    public void Configure(EntityTypeBuilder<HasarKaydi> builder)
    {
        builder.ToTable("HasarKayitlari");

        builder.Property(h => h.Aciklama).IsRequired().HasMaxLength(500);

        // Cascade: hasar kaydı ait olduğu kiralama satırının bir parçasıdır.
        builder.HasOne(h => h.KiralamaDetay)
            .WithMany(d => d.HasarKayitlari)
            .HasForeignKey(h => h.KiralamaDetayId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
