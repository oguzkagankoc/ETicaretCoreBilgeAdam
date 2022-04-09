using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImajDosyaAdi",
                table: "ETicaretUrunler");

            migrationBuilder.AddColumn<string>(
                name: "ImajDosyaUzantisi",
                table: "ETicaretUrunler",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImajDosyaUzantisi",
                table: "ETicaretUrunler");

            migrationBuilder.AddColumn<string>(
                name: "ImajDosyaAdi",
                table: "ETicaretUrunler",
                type: "nvarchar(75)",
                maxLength: 75,
                nullable: true);
        }
    }
}
