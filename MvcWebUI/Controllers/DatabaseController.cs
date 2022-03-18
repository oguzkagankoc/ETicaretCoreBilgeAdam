﻿using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace MvcWebUI.Controllers
{
    public class DatabaseController : Controller
    {
        public IActionResult Seed() // ~/Database/Seed
        {
            //ETicaretContext db = new ETicaretContext();
            //// veritabanı tablo insert işlemleri...
            //db.Dispose();
            using (ETicaretContext db = new ETicaretContext())
            {
                // verilerin silinmesi:
                var urunMagazaEntities = db.UrunMagazalar.ToList();
                db.UrunMagazalar.RemoveRange(urunMagazaEntities);

                var magazaEntities = db.Magazalar.ToList();
                db.Magazalar.RemoveRange(magazaEntities);

                var sehirEntities = db.Sehirler.ToList();
                db.Sehirler.RemoveRange(sehirEntities);

                var ulkeEntities = db.Ulkeler.ToList();
                db.Ulkeler.RemoveRange(ulkeEntities);

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
            }
            return Content("<label style=\"color:red;\"><b>İlk veriler oluşturuldu.</b></label>", "text/html", Encoding.UTF8);
            // eğer Türkçe karakterlerde sorun olursa mutlaka utf-8 akla gelmeli ve C# tarafında bunun için Encoding.UTF8 kullanılabilir.
        }
    }
}
