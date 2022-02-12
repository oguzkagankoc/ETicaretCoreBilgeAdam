using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Urunler_Kategoriler_KategoriId",
                table: "Urunler");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Urunler",
                table: "Urunler");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kategoriler",
                table: "Kategoriler");

            migrationBuilder.RenameTable(
                name: "Urunler",
                newName: "ETicaretUrunler");

            migrationBuilder.RenameTable(
                name: "Kategoriler",
                newName: "ETicaretKategoriler");

            migrationBuilder.RenameIndex(
                name: "IX_Urunler_KategoriId",
                table: "ETicaretUrunler",
                newName: "IX_ETicaretUrunler_KategoriId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ETicaretUrunler",
                table: "ETicaretUrunler",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ETicaretKategoriler",
                table: "ETicaretKategoriler",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ETicaretUrunler_ETicaretKategoriler_KategoriId",
                table: "ETicaretUrunler",
                column: "KategoriId",
                principalTable: "ETicaretKategoriler",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ETicaretUrunler_ETicaretKategoriler_KategoriId",
                table: "ETicaretUrunler");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ETicaretUrunler",
                table: "ETicaretUrunler");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ETicaretKategoriler",
                table: "ETicaretKategoriler");

            migrationBuilder.RenameTable(
                name: "ETicaretUrunler",
                newName: "Urunler");

            migrationBuilder.RenameTable(
                name: "ETicaretKategoriler",
                newName: "Kategoriler");

            migrationBuilder.RenameIndex(
                name: "IX_ETicaretUrunler_KategoriId",
                table: "Urunler",
                newName: "IX_Urunler_KategoriId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Urunler",
                table: "Urunler",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kategoriler",
                table: "Kategoriler",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Urunler_Kategoriler_KategoriId",
                table: "Urunler",
                column: "KategoriId",
                principalTable: "Kategoriler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
