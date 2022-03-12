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
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
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
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETicaretMagazalar", x => x.Id);
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
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretUrunler_KategoriId",
                table: "ETicaretUrunler",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretUrunMagazalar_MagazaId",
                table: "ETicaretUrunMagazalar",
                column: "MagazaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ETicaretUrunMagazalar");

            migrationBuilder.DropTable(
                name: "ETicaretMagazalar");

            migrationBuilder.DropTable(
                name: "ETicaretUrunler");

            migrationBuilder.DropTable(
                name: "ETicaretKategoriler");
        }
    }
}
