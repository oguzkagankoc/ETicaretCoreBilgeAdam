using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace MvcWebUI.Controllers
{
    public class DatabaseController : Controller
    {
        //[NonAction] // aksiyonun web uygulaması üzerinden çağrılmasını engeller
        public IActionResult Seed() // ~/Database/Seed
        {
            //ETicaretContext db = new ETicaretContext();
            //// veritabanı tablo insert işlemleri...
            //db.Dispose();
            using (ETicaretContext db = new ETicaretContext())
            {
                // verilerin silinmesi:
                var urunSiparisEntities = db.UrunSiparisler.ToList();
                db.UrunSiparisler.RemoveRange(urunSiparisEntities);

                var siparisEntities = db.Siparisler.ToList();
                db.Siparisler.RemoveRange(siparisEntities);

                var kullaniciDetayiEntities = db.KullaniciDetaylari.ToList();
                db.KullaniciDetaylari.RemoveRange(kullaniciDetayiEntities);

                var kullaniciEntities = db.Kullanicilar.ToList();
                db.Kullanicilar.RemoveRange(kullaniciEntities);

                var rolEntities = db.Roller.ToList();
                db.Roller.RemoveRange(rolEntities);

                var sehirEntities = db.Sehirler.ToList();
                db.Sehirler.RemoveRange(sehirEntities);

                var ulkeEntities = db.Ulkeler.ToList();
                db.Ulkeler.RemoveRange(ulkeEntities);

                var urunMagazaEntities = db.UrunMagazalar.ToList();
                db.UrunMagazalar.RemoveRange(urunMagazaEntities);

                var magazaEntities = db.Magazalar.ToList();
                db.Magazalar.RemoveRange(magazaEntities);

                var urunEntities = db.Urunler.ToList();

                //if (urunEntities != null && urunEntities.Count > 0)
                //{
                //    foreach (var urunEntity in urunEntities)
                //    {
                //        db.Urunler.Remove(urunEntity);
                //    }
                //}
                db.Urunler.RemoveRange(urunEntities);

                var kategoriEntities = db.Kategoriler.ToList();
                db.Kategoriler.RemoveRange(kategoriEntities);

                // eğer istenirse tablo id'leri 1'den başlatılabilir:
                // eğer tablolarda veri yoksa id 0'dan başlayabilir bu yüzden başlangıç tablosu olan Kategoriler tablosunda veri varsa bu kısmı çalıştırıyoruz!
                if (kategoriEntities.Count > 0)
                {
                    db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('ETicaretUrunler', RESEED, 0)");
                    db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('ETicaretMagazalar', RESEED, 0)");
                    db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('ETicaretKategoriler', RESEED, 0)");
                    db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('ETicaretKullanicilar', RESEED, 0)");
                    db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('ETicaretSehirler', RESEED, 0)");
                    db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('ETicaretUlkeler', RESEED, 0)");
                    db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('ETicaretRoller', RESEED, 0)");
                    db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('ETicaretSiparisler', RESEED, 0)");
                }

                // verilerin eklenmesi:
                db.Kategoriler.Add(new Kategori()
                {
                    Adi = "Bilgisayar",
                    Urunler = new List<Urun>()
                    {
                        new Urun()
                        {
                            Adi = "Dizüstü Bilgisayar",
                            BirimFiyati = 3000.5,
                            StokMiktari = 10,
                            SonKullanmaTarihi = new DateTime(2032, 1, 27)
                        },
                        new Urun()
                        {
                            Adi = "Bilgisayar Faresi",
                            BirimFiyati = 20.5,
                            StokMiktari = 20,
                            SonKullanmaTarihi = DateTime.Parse("19.05.2027", new CultureInfo("tr-TR")) 
                            // İngilizce bölgesel ayar için: en-US, sadece tarih ve ondalık veri tipleri için CultureInfo kullanılmalı,
                            // ~/Program.cs içersinde tüm uygulama için tek seferde AppCore üzerinden tanımlanıp kullanılabilir.
                        },
                        new Urun()
                        {
                            Adi = "Bilgisayar Klavyesi",
                            BirimFiyati = 40,
                            StokMiktari = 21,
                            Aciklamasi = "Bilgisayar Bileşeni"
                        },
                         new Urun()
                        {
                            Adi = "Bilgisayar Monitörü",
                            BirimFiyati = 2500,
                            StokMiktari = 27,
                            Aciklamasi = "Bilgisayar Bileşeni"
                        }
                    }
                });
                db.Kategoriler.Add(new Kategori()
                {
                    Adi = "Ev Eğlence Sistemi",
                    Aciklamasi = "Ev Sinema Sistemleri ve Televizyonlar",
                    Urunler = new List<Urun>()
                    {
                        new Urun()
                        {
                            Adi = "Hoparlör",
                            BirimFiyati = 2500,
                            StokMiktari = 5
                        },
                        new Urun()
                        {
                            Adi = "Amfi",
                            BirimFiyati = 5000,
                            StokMiktari = 9
                        },
                        new Urun()
                        {
                            Adi = "Ekolayzer",
                            BirimFiyati = 1000,
                            StokMiktari = 11
                        }
                    }
                });

                db.SaveChanges();

                db.Magazalar.Add(new Magaza()
                {
                    Adi = "Vatan Bilgisayar",
                    SanalMi = false,
                    UrunMagazalar = new List<UrunMagaza>()
                    {
                        new UrunMagaza()
                        {
                            UrunId = db.Urunler.SingleOrDefault(u => u.Adi == "Dizüstü Bilgisayar").Id
                        },
                        new UrunMagaza()
                        {
                            UrunId = db.Urunler.SingleOrDefault(u => u.Adi == "Amfi").Id
                        },
                        new UrunMagaza()
                        {
                            UrunId = db.Urunler.SingleOrDefault(u => u.Adi == "Ekolayzer").Id
                        }
                    }
                });
                db.Magazalar.Add(new Magaza()
                {
                    Adi = "Hepsiburada",
                    SanalMi = true,
                    UrunMagazalar = new List<UrunMagaza>()
                    {
                        new UrunMagaza()
                        {
                            UrunId = db.Urunler.SingleOrDefault(u => u.Adi == "Bilgisayar Faresi").Id
                        },
                        new UrunMagaza()
                        {
                            UrunId = db.Urunler.SingleOrDefault(u => u.Adi == "Bilgisayar Klavyesi").Id
                        },
                        new UrunMagaza()
                        {
                            UrunId = db.Urunler.SingleOrDefault(u => u.Adi == "Bilgisayar Monitörü").Id
                        },
                        new UrunMagaza()
                        {
                            UrunId = db.Urunler.SingleOrDefault(u => u.Adi == "Hoparlör").Id
                        }
                    }
                });
                db.Ulkeler.Add(new Ulke()
                {
                    Adi = "Türkiye",
                    Sehirler = new List<Sehir>()
                    {
                        new Sehir()
                        {
                            Adi = "Ankara"
                        },
                        new Sehir()
                        {
                            Adi = "İstanbul"
                        },
                        new Sehir()
                        {
                            Adi = "İzmir"
                        }
                    }
                });
                db.Ulkeler.Add(new Ulke()
                {
                    Adi = "Amerika Birleşik Devletleri",
                    Sehirler = new List<Sehir>()
                    {
                        new Sehir()
                        {
                            Adi = "New York"
                        },
                        new Sehir()
                        {
                            Adi = "Los Angeles"
                        }
                    }
                });

                db.SaveChanges();

                db.Roller.Add(new Rol()
                {
                    Adi = "Admin",
                    Kullanicilar = new List<Kullanici>()
                    {
                        new Kullanici()
                        {
                            KullaniciAdi = "cagil",
                            Sifre = "cagil",
                            AktifMi = true,
                            KullaniciDetayi = new KullaniciDetayi()
                            {
                                Adres = "Çankaya",
                                Cinsiyet = Cinsiyet.Erkek,
                                Eposta = "cagil@eticaret.com",
                                UlkeId = db.Ulkeler.SingleOrDefault(u => u.Adi == "Türkiye").Id,
                                SehirId = db.Sehirler.SingleOrDefault(s => s.Adi == "Ankara").Id
                            }
                        }
                    }
                });
                db.Roller.Add(new Rol()
                {
                    Adi = "Kullanıcı",
                    Kullanicilar = new List<Kullanici>()
                    {
                        new Kullanici()
                        {
                            KullaniciAdi = "leo",
                            Sifre = "leo",
                            AktifMi = true,
                            KullaniciDetayi = new KullaniciDetayi()
                            {
                                Adres = "Çankaya",
                                Cinsiyet = Cinsiyet.Erkek,
                                Eposta = "leo@eticaret.com",
                                UlkeId = db.Ulkeler.SingleOrDefault(u => u.Adi == "Türkiye").Id,
                                SehirId = db.Sehirler.SingleOrDefault(s => s.Adi == "Ankara").Id
                            }
                        }
                    }
                });

                db.SaveChanges();
            }
            return Content("<label style=\"color:red;\"><b>İlk veriler oluşturuldu.</b></label>", "text/html", Encoding.UTF8);
            // eğer Türkçe karakterlerde sorun olursa mutlaka utf-8 akla gelmeli ve C# tarafında bunun için Encoding.UTF8 kullanılabilir.
        }
    }
}
