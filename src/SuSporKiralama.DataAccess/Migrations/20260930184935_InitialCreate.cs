using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuSporKiralama.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ekipmanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Marka = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BirimUcret = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    DepozitoTutari = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    EkipmanTipi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Beden = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    KisiKapasitesi = table.Column<int>(type: "int", nullable: true),
                    UzunlukCm = table.Column<int>(type: "int", nullable: true),
                    MaxTasimaKg = table.Column<int>(type: "int", nullable: true),
                    Tip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ekipmanlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Musteriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Eposta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeneyimSeviyesi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notlar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Musteriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personeller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SifreHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personeller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kiralamalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MusteriId = table.Column<int>(type: "int", nullable: false),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    BaslangicZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanlananBitisZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GercekBitisZamani = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Durum = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ToplamUcret = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kiralamalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kiralamalar_Musteriler_MusteriId",
                        column: x => x.MusteriId,
                        principalTable: "Musteriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kiralamalar_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KiralamaDetaylari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KiralamaId = table.Column<int>(type: "int", nullable: false),
                    EkipmanId = table.Column<int>(type: "int", nullable: false),
                    UygulananBirimUcret = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    HesaplananUcret = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KiralamaDetaylari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KiralamaDetaylari_Ekipmanlar_EkipmanId",
                        column: x => x.EkipmanId,
                        principalTable: "Ekipmanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KiralamaDetaylari_Kiralamalar_KiralamaId",
                        column: x => x.KiralamaId,
                        principalTable: "Kiralamalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Odemeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KiralamaId = table.Column<int>(type: "int", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    OdemeTipi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OdemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Odemeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Odemeler_Kiralamalar_KiralamaId",
                        column: x => x.KiralamaId,
                        principalTable: "Kiralamalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HasarKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KiralamaDetayId = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HasarBedeli = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HasarKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HasarKayitlari_KiralamaDetaylari_KiralamaDetayId",
                        column: x => x.KiralamaDetayId,
                        principalTable: "KiralamaDetaylari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ekipmanlar_Kod",
                table: "Ekipmanlar",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HasarKayitlari_KiralamaDetayId",
                table: "HasarKayitlari",
                column: "KiralamaDetayId");

            migrationBuilder.CreateIndex(
                name: "IX_KiralamaDetaylari_EkipmanId",
                table: "KiralamaDetaylari",
                column: "EkipmanId");

            migrationBuilder.CreateIndex(
                name: "IX_KiralamaDetaylari_KiralamaId_EkipmanId",
                table: "KiralamaDetaylari",
                columns: new[] { "KiralamaId", "EkipmanId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kiralamalar_Durum_BaslangicZamani",
                table: "Kiralamalar",
                columns: new[] { "Durum", "BaslangicZamani" });

            migrationBuilder.CreateIndex(
                name: "IX_Kiralamalar_MusteriId",
                table: "Kiralamalar",
                column: "MusteriId");

            migrationBuilder.CreateIndex(
                name: "IX_Kiralamalar_PersonelId",
                table: "Kiralamalar",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_Musteriler_Telefon",
                table: "Musteriler",
                column: "Telefon",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Odemeler_KiralamaId",
                table: "Odemeler",
                column: "KiralamaId");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_KullaniciAdi",
                table: "Personeller",
                column: "KullaniciAdi",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HasarKayitlari");

            migrationBuilder.DropTable(
                name: "Odemeler");

            migrationBuilder.DropTable(
                name: "KiralamaDetaylari");

            migrationBuilder.DropTable(
                name: "Ekipmanlar");

            migrationBuilder.DropTable(
                name: "Kiralamalar");

            migrationBuilder.DropTable(
                name: "Musteriler");

            migrationBuilder.DropTable(
                name: "Personeller");
        }
    }
}
