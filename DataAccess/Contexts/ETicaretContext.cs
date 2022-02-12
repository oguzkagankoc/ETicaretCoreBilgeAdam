using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Contexts
{
    public class ETicaretContext : DbContext
    {
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Windows Authentication
            //string connectionString = "server=.;database=BA_ETicaretCore;trusted_connection=true;multipleactiveresultsets=true;";

            // SQL Server Authentication
            string connectionString = "server=.;database=BA_ETicaretCore;user id=sa;password=123;multipleactiveresultsets=true;";

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Urun>()
                .ToTable("ETicaretUrunler")
                .HasOne(urun => urun.Kategori)
                .WithMany(kategori => kategori.Urunler)
                .HasForeignKey(urun => urun.KategoriId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Kategori>()
                .ToTable("ETicaretKategoriler");
        }
    }
}
