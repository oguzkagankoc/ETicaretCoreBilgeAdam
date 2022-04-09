using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImajDosyaYolu",
                table: "ETicaretUrunler");

            migrationBuilder.AddColumn<string>(
                name: "ImajDosyaAdi",
                table: "ETicaretUrunler",
                type: "nvarchar(75)",
                maxLength: 75,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImajDosyaAdi",
                table: "ETicaretUrunler");

            migrationBuilder.AddColumn<string>(
                name: "ImajDosyaYolu",
                table: "ETicaretUrunler",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
