using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Siparis",
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
                    table.PrimaryKey("PK_Siparis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Siparis_ETicaretKullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "ETicaretKullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_ETicaretUrunSiparisler_ETicaretUrunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "ETicaretUrunler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ETicaretUrunSiparisler_Siparis_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparis",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ETicaretUrunSiparisler_SiparisId",
                table: "ETicaretUrunSiparisler",
                column: "SiparisId");

            migrationBuilder.CreateIndex(
                name: "IX_Siparis_KullaniciId",
                table: "Siparis",
                column: "KullaniciId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ETicaretUrunSiparisler");

            migrationBuilder.DropTable(
                name: "Siparis");
        }
    }
}
