using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuSporKiralama.Entities;

namespace SuSporKiralama.DataAccess.Konfigurasyonlar;

public class OdemeConfiguration : IEntityTypeConfiguration<Odeme>
{
    public void Configure(EntityTypeBuilder<Odeme> builder)
    {
        builder.ToTable("Odemeler");

        builder.Property(o => o.Aciklama).HasMaxLength(250);

        // Restrict: ödeme mali bir kayıttır; ödemesi olan kiralama yanlışlıkla silinip
        // ödemeler kaybolmasın.
        builder.HasOne(o => o.Kiralama)
            .WithMany(k => k.Odemeler)
            .HasForeignKey(o => o.KiralamaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
