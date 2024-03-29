﻿using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MvcWebUI.Controllers
{
    [Authorize]
    public class SepetController : Controller
    {
        private readonly IUrunService _urunService;

        public SepetController(IUrunService urunService)
        {
            _urunService = urunService;
        }

        public IActionResult Ekle(int? urunId)
        {
            if (urunId == null)
                return View("Hata", "Ürün Id gereklidir!");
            UrunModel urun = _urunService.Query().SingleOrDefault(u => u.Id == urunId);
            if (urun == null)
                return View("Hata", "Ürün bulunamadı!");

            // eğer stokta ürün varsa sepete eklenebilsin
            if (urun.StokMiktari == 0)
            {
                TempData["Sonuc"] = "Sepete eklenmek istenen ürün stokta bulunmamaktadır!";
                return RedirectToAction("Index", "Urunler", new { sepetUrunId = urun.Id });
            }

            SepetElemanModel eleman;
            List<SepetElemanModel> sepet;
            string sepetJson;
            sepet = SessiondanSepetiGetir();
            eleman = new SepetElemanModel()
            {
                UrunId = urunId.Value,
                KullaniciId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value),
                UrunAdi = urun.Adi,
                UrunBirimFiyati = urun.BirimFiyati ?? 0
            };
            sepet.Add(eleman);
            int urunAdedi = sepet.Count(s => s.UrunId == urunId);

            // eklenen ürünün adedi ürün stok miktarini aşmamalı
            if (urunAdedi > urun.StokMiktari)
            {
                TempData["Sonuc"] = "Sepete eklenen ürün adedi ürün stok miktarından fazladır!";
            }
            else
            {
                sepetJson = JsonConvert.SerializeObject(sepet);
                // Session'da asla kullanıcı bilgileri gibi kritik veriler tutulmamalıdır!
                HttpContext.Session.SetString("sepet", sepetJson);
                TempData["Sonuc"] = urunAdedi + " adet " + eleman.UrunAdi + " sepete eklendi.";
            }
            
            return RedirectToAction("Index", "Urunler", new { sepetUrunId = eleman.UrunId });
        }

        private List<SepetElemanModel> SessiondanSepetiGetir()
        {
            List<SepetElemanModel> sepet = new List<SepetElemanModel>();
            string sepetJson = HttpContext.Session.GetString("sepet");
            if (!string.IsNullOrWhiteSpace(sepetJson)) // byte[], int veya string olarak session'da veri kullanılabilir
            {
                sepet = JsonConvert.DeserializeObject<List<SepetElemanModel>>(sepetJson);
            }
            return sepet;
        }

        public IActionResult Getir()
        {
            List<SepetElemanModel> sepet = SessiondanSepetiGetir();

            // session'dan alınan sepetin sisteme giriş yapan kullanıcı Id'sine göre filtrelenmesi:
            sepet = sepet.Where(s => s.KullaniciId == Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value)).ToList();

            List<SepetElemanGroupByModel> sepetGroupBy = (from s in sepet
                                                              //group s by s.UrunAdi // tek bir özellik gruplanıyorsa kullanılır
                                                          group s by new { s.UrunId, s.KullaniciId, s.UrunAdi } // birden çok özellik gruplanıyorsa kullanılır
                                                         into sGroupBy
                                                          select new SepetElemanGroupByModel()
                                                          {
                                                              UrunId = sGroupBy.Key.UrunId,
                                                              KullaniciId = sGroupBy.Key.KullaniciId,
                                                              UrunAdi = sGroupBy.Key.UrunAdi,
                                                              ToplamUrunBirimFiyati = sGroupBy.Sum(sgb => sgb.UrunBirimFiyati),
                                                              ToplamUrunBirimFiyatiDisplay = sGroupBy.Sum(sgb => sgb.UrunBirimFiyati).ToString("C2"),
                                                              ToplamUrunAdedi = sGroupBy.Count()
                                                          }).ToList();

            sepetGroupBy = sepetGroupBy.OrderBy(s => s.UrunAdi).ToList();

            //return View(sepet);
            return View("GetirGroupBy", sepetGroupBy);
        }

        public IActionResult Temizle()
        {
            // kullanıcıdan bağımsız session'daki sepet anahtarına sahip tüm verilerin silinmesi:
            //HttpContext.Session.Remove("sepet");

            // kullanıcıya ait session'daki sepet anahtarına sahip verilerin silinmesi:
            List<SepetElemanModel> sepet = new List<SepetElemanModel>();
            string sepetJson = HttpContext.Session.GetString("sepet");
            if (!string.IsNullOrWhiteSpace(sepetJson))
            {
                sepet = JsonConvert.DeserializeObject<List<SepetElemanModel>>(sepetJson);
            }
            // session'dan alınan sepetin sisteme giriş yapan kullanıcı Id'sine göre filtrelenmesi:
            List<SepetElemanModel> kullaniciSepeti = sepet.Where(s => s.KullaniciId == Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value)).ToList();
            foreach (SepetElemanModel eleman in kullaniciSepeti)
            {
                sepet.Remove(eleman);
            }
            sepetJson = JsonConvert.SerializeObject(sepet);
            HttpContext.Session.SetString("sepet", sepetJson);

            TempData["Sonuc"] = "Sepet temizlendi.";
            return RedirectToAction(nameof(Getir));
        }

        public IActionResult Sil(int? urunId, int? kullaniciId)
        {
            if (!urunId.HasValue || !kullaniciId.HasValue)
                return View("Hata", "Ürün Id ve Kullanıcı Id gereklidir!");
            List<SepetElemanModel> sepet = SessiondanSepetiGetir();
            SepetElemanModel eleman = sepet.FirstOrDefault(s => s.UrunId == urunId.Value && s.KullaniciId == kullaniciId.Value);
            if (eleman != null)
            {
                sepet.Remove(eleman);
                HttpContext.Session.SetString("sepet", JsonConvert.SerializeObject(sepet));
                TempData["Sonuc"] = "Ürün sepetten silindi.";
            }
            return RedirectToAction(nameof(Getir));
        }
    }
}
