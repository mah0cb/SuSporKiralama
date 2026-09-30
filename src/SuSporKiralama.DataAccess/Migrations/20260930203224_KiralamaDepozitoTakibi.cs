using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuSporKiralama.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class KiralamaDepozitoTakibi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepozitoDurumu",
                table: "Kiralamalar",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Alinmadi");

            migrationBuilder.AddColumn<decimal>(
                name: "DepozitoMahsupTutari",
                table: "Kiralamalar",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DepozitoTutari",
                table: "Kiralamalar",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            // Mevcut kayıtlar: depozito, ekipmanların bugünkü depozito tutarlarının toplamı sayılır
            // (kiralama anındaki değer kaydedilmemişti). Aktif kiralamada depozito alınmıştır.
            // Tamamlanmış eski kiralamalarda hasar ödemelere dahil edilmişti; bu yüzden depozito
            // tamamen iade edilmiş sayılır (mahsup 0) ve kalan borç değişmez.
            migrationBuilder.Sql("""
                UPDATE k
                SET k.DepozitoTutari = t.Toplam,
                    k.DepozitoDurumu = CASE k.Durum WHEN 'Aktif' THEN 'Alindi' ELSE 'IadeEdildi' END
                FROM Kiralamalar k
                JOIN (SELECT d.KiralamaId, SUM(e.DepozitoTutari) AS Toplam
                      FROM KiralamaDetaylari d
                      JOIN Ekipmanlar e ON e.Id = d.EkipmanId
                      GROUP BY d.KiralamaId) t ON t.KiralamaId = k.Id
                WHERE k.Durum IN ('Aktif', 'Tamamlandi');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepozitoDurumu",
                table: "Kiralamalar");

            migrationBuilder.DropColumn(
                name: "DepozitoMahsupTutari",
                table: "Kiralamalar");

            migrationBuilder.DropColumn(
                name: "DepozitoTutari",
                table: "Kiralamalar");
        }
    }
}
