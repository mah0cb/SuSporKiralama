using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuSporKiralama.Entities;

namespace SuSporKiralama.DataAccess.Konfigurasyonlar;

public class KiralamaDetayConfiguration : IEntityTypeConfiguration<KiralamaDetay>
{
    public void Configure(EntityTypeBuilder<KiralamaDetay> builder)
    {
        builder.ToTable("KiralamaDetaylari");

        // Cascade: detay satırı kiralamasız anlamsızdır; kiralama silinirse satırları da silinir.
        builder.HasOne(d => d.Kiralama)
            .WithMany(k => k.Detaylar)
            .HasForeignKey(d => d.KiralamaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict: kiralanmış ekipman silinemez (geçmiş korunur); kullanımdan kalkınca
        // Durum = HizmetDisi yapılır.
        builder.HasOne(d => d.Ekipman)
            .WithMany(e => e.KiralamaDetaylari)
            .HasForeignKey(d => d.EkipmanId)
            .OnDelete(DeleteBehavior.Restrict);

        // Aynı ekipman aynı kiralamaya iki kez eklenemez.
        builder.HasIndex(d => new { d.KiralamaId, d.EkipmanId }).IsUnique();
    }
}
