using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Contexts
{
    public class ETicaretContext : DbContext
    {
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Magaza> Magazalar { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Windows Authentication
            //string connectionString = "server=.;database=BA_ETicaretCore;trusted_connection=true;multipleactiveresultsets=true;";

            // SQL Server Authentication
            //string connectionString = "server=.\\SQLEXPRESS;database=BA_ETicaretCore;user id=sa;password=sa;multipleactiveresultsets=true;";
            string connectionString = "server=.;database=BA_ETicaretCore;user id=sa;password=123;multipleactiveresultsets=true;";

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Urun>()
                .ToTable("ETicaretUrunler") // genelde tablo isimlerini değiştirmeye gerek yoktur, DbSet ismini kullanır (Urunler)
                .HasOne(urun => urun.Kategori)
                .WithMany(kategori => kategori.Urunler)
                .HasForeignKey(urun => urun.KategoriId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Urun>()
                .HasMany(urun => urun.UrunMagazalar)
                .WithOne(urunMagaza => urunMagaza.Urun)
                .HasForeignKey(urunMagaza => urunMagaza.UrunId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Magaza>()
                .ToTable("ETicaretMagazalar")
                .HasMany(magaza => magaza.UrunMagazalar)
                .WithOne(urunMagaza => urunMagaza.Magaza)
                .HasForeignKey(urunMagaza => urunMagaza.MagazaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Kategori>()
                .ToTable("ETicaretKategoriler");

            modelBuilder.Entity<UrunMagaza>()
                .ToTable("ETicaretUrunMagazalar")
                .HasKey(urunMagaza => new { urunMagaza.UrunId, urunMagaza.MagazaId });
        }
    }
}
