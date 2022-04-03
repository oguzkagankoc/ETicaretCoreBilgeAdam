using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ETicaretKategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklamasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETicaretKategoriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ETicaretMagazalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SanalMi = table.Column<bool>(type: "bit", nullable: false),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETicaretMagazalar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ETicaretRoller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETicaretRoller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ETicaretUlkeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETicaretUlkeler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ETicaretUrunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aciklamasi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BirimFiyati = table.Column<double>(type: "float", nullable: false),
                    StokMiktari = table.Column<int>(type: "int", nullable: false),
                    SonKullanmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KategoriId = table.Column<int>(type: "int", nullable: false),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETicaretUrunler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ETicaretUrunler_ETicaretKategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "ETicaretKategoriler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ETicaretKullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETicaretKullanicilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ETicaretKullanicilar_ETicaretRoller_RolId",
                        column: x => x.RolId,
                        principalTable: "ETicaretRoller",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ETicaretSehirler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UlkeId = table.Column<int>(type: "int", nullable: false),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETicaretSehirler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ETicaretSehirler_ETicaretUlkeler_UlkeId",
                        column: x => x.UlkeId,
                        principalTable: "ETicaretUlkeler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ETicaretUrunMagazalar",
                columns: table => new
                {
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    MagazaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETicaretUrunMagazalar", x => new { x.UrunId, x.MagazaId });
                    table.ForeignKey(
                        name: "FK_ETicaretUrunMagazalar_ETicaretMagazalar_MagazaId",
                        column: x => x.MagazaId,
                        principalTable: "ETicaretMagazalar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ETicaretUrunMagazalar_ETicaretUrunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "ETicaretUrunler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ETicaretSiparisler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETicaretSiparisler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ETicaretSiparisler_ETicaretKullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "ETicaretKullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ETicaretKullaniciDetaylari",
                columns: table => new
                {
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Cinsiyet = table.Column<int>(type: "int", nullable: false),
                    Eposta = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UlkeId = table.Column<int>(type: "int", nullable: false),
                    SehirId = table.Column<int>(type: "int", nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETicaretKullaniciDetaylari", x => x.KullaniciId);
                    table.ForeignKey(
                        name: "FK_ETicaretKullaniciDetaylari_ETicaretKullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "ETicaretKullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ETicaretKullaniciDetaylari_ETicaretSehirler_SehirId",
                        column: x => x.SehirId,
                        principalTable: "ETicaretSehirler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ETicaretKullaniciDetaylari_ETicaretUlkeler_UlkeId",
                        column: x => x.UlkeId,
                        principalTable: "ETicaretUlkeler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ETicaretUrunSiparisler",
                columns: table => new
                {
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    SiparisId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETicaretUrunSiparisler", x => new { x.UrunId, x.SiparisId });
                    table.ForeignKey(
                        name: "FK_ETicaretUrunSiparisler_ETicaretSiparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "ETicaretSiparisler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ETicaretUrunSiparisler_ETicaretUrunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "ETicaretUrunler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretKullaniciDetaylari_Eposta",
                table: "ETicaretKullaniciDetaylari",
                column: "Eposta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretKullaniciDetaylari_SehirId",
                table: "ETicaretKullaniciDetaylari",
                column: "SehirId");

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretKullaniciDetaylari_UlkeId",
                table: "ETicaretKullaniciDetaylari",
                column: "UlkeId");

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretKullanicilar_KullaniciAdi",
                table: "ETicaretKullanicilar",
                column: "KullaniciAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretKullanicilar_RolId",
                table: "ETicaretKullanicilar",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretSehirler_UlkeId",
                table: "ETicaretSehirler",
                column: "UlkeId");

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretSiparisler_KullaniciId",
                table: "ETicaretSiparisler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretUrunler_Adi",
                table: "ETicaretUrunler",
                column: "Adi");

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretUrunler_KategoriId",
                table: "ETicaretUrunler",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretUrunMagazalar_MagazaId",
                table: "ETicaretUrunMagazalar",
                column: "MagazaId");

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretUrunSiparisler_SiparisId",
                table: "ETicaretUrunSiparisler",
                column: "SiparisId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ETicaretKullaniciDetaylari");

            migrationBuilder.DropTable(
                name: "ETicaretUrunMagazalar");

            migrationBuilder.DropTable(
                name: "ETicaretUrunSiparisler");

            migrationBuilder.DropTable(
                name: "ETicaretSehirler");

            migrationBuilder.DropTable(
                name: "ETicaretMagazalar");

            migrationBuilder.DropTable(
                name: "ETicaretSiparisler");

            migrationBuilder.DropTable(
                name: "ETicaretUrunler");

            migrationBuilder.DropTable(
                name: "ETicaretUlkeler");

            migrationBuilder.DropTable(
                name: "ETicaretKullanicilar");

            migrationBuilder.DropTable(
                name: "ETicaretKategoriler");

            migrationBuilder.DropTable(
                name: "ETicaretRoller");
        }
    }
}
