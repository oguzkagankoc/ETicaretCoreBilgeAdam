﻿using AppCore.DataAccess.Configs;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Contexts
{
    public class ETicaretContext : DbContext
    {
        // tüm entity'ler için DbSet özellikleri oluşturulmalı
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Magaza> Magazalar { get; set; }
        public DbSet<UrunMagaza> UrunMagazalar { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<KullaniciDetayi> KullaniciDetaylari { get; set; }
        public DbSet<Rol> Roller { get; set; }
        public DbSet<Ulke> Ulkeler { get; set; }
        public DbSet<Sehir> Sehirler { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<UrunSiparis> UrunSiparisler { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Windows Authentication
            //string connectionString = "server=.;database=BA_ETicaretCore;trusted_connection=true;multipleactiveresultsets=true;";

            // SQL Server Authentication
            //string connectionString = "server=.\\SQLEXPRESS;database=BA_ETicaretCore;user id=sa;password=sa;multipleactiveresultsets=true;";
            //string connectionString = "server=.;database=BA_ETicaretCore;user id=sa;password=123;multipleactiveresultsets=true;";

            // appsettings.json'dan gelen connection string kullanımı:
            string connectionString;
            if (string.IsNullOrWhiteSpace(ConnectionConfig.ConnectionString)) // Scaffolding işleminde ConnectionConfig.ConnectionString null geldiğinden kendimiz dev veritabanı bağlantı bilgisini kullanıyoruz
            {
                ConnectionConfig.ConnectionString = "server=.\\SQLEXPRESS;database=BA_ETicaretCore;user id=sa;password=sa;multipleactiveresultsets=true;";
                //ConnectionConfig.ConnectionString = "server=.;database=BA_ETicaretCore;user id=sa;password=123;multipleactiveresultsets=true;";
            }
            connectionString = ConnectionConfig.ConnectionString;
            
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

            modelBuilder.Entity<Kullanici>()
                .ToTable("ETicaretKullanicilar")
                .HasOne(kullanici => kullanici.Rol)
                .WithMany(rol => rol.Kullanicilar)
                .HasForeignKey(kullanici => kullanici.RolId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<KullaniciDetayi>()
                .ToTable("ETicaretKullaniciDetaylari")
                .HasOne(kullaniciDetayi => kullaniciDetayi.Kullanici)
                .WithOne(kullanici => kullanici.KullaniciDetayi)
                .HasForeignKey<KullaniciDetayi>(kullaniciDetayi => kullaniciDetayi.KullaniciId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<KullaniciDetayi>()
                .HasOne(kullaniciDetayi => kullaniciDetayi.Ulke)
                .WithMany(ulke => ulke.KullaniciDetaylari)
                .HasForeignKey(kullaniciDetayi => kullaniciDetayi.UlkeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<KullaniciDetayi>()
                .HasOne(kullaniciDetayi => kullaniciDetayi.Sehir)
                .WithMany(sehir => sehir.KullaniciDetaylari)
                .HasForeignKey(kullaniciDetayi => kullaniciDetayi.SehirId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Sehir>()
                .ToTable("ETicaretSehirler")
                .HasOne(sehir => sehir.Ulke)
                .WithMany(ulke => ulke.Sehirler)
                .HasForeignKey(sehir => sehir.UlkeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UrunSiparis>()
                .ToTable("ETicaretUrunSiparisler")
                .HasOne(urunSiparis => urunSiparis.Urun)
                .WithMany(urun => urun.UrunSiparisler)
                .HasForeignKey(urunSiparis => urunSiparis.UrunId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UrunSiparis>()
                .HasOne(urunSiparis => urunSiparis.Siparis)
                .WithMany(siparis => siparis.UrunSiparisler)
                .HasForeignKey(urunSiparis => urunSiparis.SiparisId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UrunSiparis>()
                .HasKey(urunSiparis => new { urunSiparis.UrunId, urunSiparis.SiparisId });

            modelBuilder.Entity<Ulke>().ToTable("ETicaretUlkeler");

            modelBuilder.Entity<Rol>().ToTable("ETicaretRoller");

            modelBuilder.Entity<Siparis>().ToTable("ETicaretSiparisler");

            modelBuilder.Entity<Urun>().HasIndex(urun => urun.Adi);

            modelBuilder.Entity<Kullanici>().HasIndex(kullanici => kullanici.KullaniciAdi).IsUnique();

            modelBuilder.Entity<KullaniciDetayi>().HasIndex(kullaniciDetay => kullaniciDetay.Eposta).IsUnique();
        }
    }
}
