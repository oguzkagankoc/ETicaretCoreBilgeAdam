using System.Globalization;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Text;
using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace MvcWebUI.Controllers
{
    public class HesaplarController : Controller
    {
        private readonly IHesapService _hesapService;

        private readonly IUlkeService _ulkeService;
        private readonly ISehirService _sehirService;

        public HesaplarController(IHesapService hesapService, IUlkeService ulkeService, ISehirService sehirService)
        {
            _hesapService = hesapService;
            _ulkeService = ulkeService;
            _sehirService = sehirService;
        }

        public IActionResult Giris()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Giris(KullaniciGirisModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _hesapService.Giris(model);
                if (result.IsSuccessful)
                {
                    #region Demo için her admin kullanıcısı girişi yapıldığında verilerin sıfırlanması
                    if (result.Data.RolId == (int)Business.Enums.Rol.Admin)
                        Seed();
                    #endregion

                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, result.Data.KullaniciAdi),
                        new Claim(ClaimTypes.Role, result.Data.RolAdiDisplay),
                        //new Claim(ClaimTypes.Email, result.Data.KullaniciDetayi.Eposta), // istenirse Email gibi claim tipleri üzerinden kullanıcının e-posta'sı gibi claim'ler eklenebilir
                        new Claim(ClaimTypes.Sid, result.Data.Id.ToString())
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", result.Message);
            }
            return View(model);
        }

        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult YetkisizIslem()
        {
            return View("Hata", "Bu işlem için yetkiniz bulunmamaktadır!");
        }

        public IActionResult Kayit()
        {
            var result = _ulkeService.UlkeleriGetir();
            if (result.IsSuccessful)
                ViewBag.UlkeId = new SelectList(result.Data, "Id", "Adi");
            else
                ViewBag.Mesaj = result.Message;
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Kayit(KullaniciKayitModel model)
        {
            if (ModelState.IsValid)
            {
                var kayitSonuc = _hesapService.Kayit(model);
                if (kayitSonuc.IsSuccessful)
                    return RedirectToAction(nameof(Giris));
                ViewBag.Mesaj = kayitSonuc.Message;
            }
            var ulkeSonuc = _ulkeService.UlkeleriGetir();
            ViewBag.UlkeId = new SelectList(ulkeSonuc.Data, "Id", "Adi", model.KullaniciDetayi.UlkeId ?? -1); // eğer UlkeId null'sa -1 kullan
            var sehirSonuc = _sehirService.SehirleriGetir(model.KullaniciDetayi.UlkeId ?? -1);
            ViewBag.SehirId = new SelectList(sehirSonuc.Data, "Id", "Adi", model.KullaniciDetayi.SehirId ?? -1);
            return View(model);
        }

        #region Demo için her admin kullanıcısı girişi yapıldığında verilerin sıfırlanması
        private void Seed()
        {
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

               db.Urunler.RemoveRange(urunEntities);

                var kategoriEntities = db.Kategoriler.ToList();
                db.Kategoriler.RemoveRange(kategoriEntities);

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
        }
        #endregion
    }
}
