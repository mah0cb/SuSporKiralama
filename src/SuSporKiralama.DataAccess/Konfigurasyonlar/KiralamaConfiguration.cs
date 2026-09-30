using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuSporKiralama.Entities;

namespace SuSporKiralama.DataAccess.Konfigurasyonlar;

public class KiralamaConfiguration : IEntityTypeConfiguration<Kiralama>
{
    public void Configure(EntityTypeBuilder<Kiralama> builder)
    {
        builder.ToTable("Kiralamalar");

        // Restrict: kiralama geçmişi olan müşteri silinemez. Geçmiş kaybolmasın diye
        // müşteri silinmek yerine AktifMi = false ile pasife alınır.
        builder.HasOne(k => k.Musteri)
            .WithMany(m => m.Kiralamalar)
            .HasForeignKey(k => k.MusteriId)
            .OnDelete(DeleteBehavior.Restrict);

        // Restrict: kiralamayı hangi personelin yaptığı bilgisi korunmalı; personel pasife alınır.
        builder.HasOne(k => k.Personel)
            .WithMany(p => p.Kiralamalar)
            .HasForeignKey(k => k.PersonelId)
            .OnDelete(DeleteBehavior.Restrict);

        // Müsaitlik ve rapor sorguları durum ve tarihe göre filtreleyecek.
        builder.HasIndex(k => new { k.Durum, k.BaslangicZamani });
    }
}
