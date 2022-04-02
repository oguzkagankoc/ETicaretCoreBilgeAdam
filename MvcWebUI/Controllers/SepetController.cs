using Business.Models;
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
            List<SepetElemanModel> sepet = new List<SepetElemanModel>();
            SepetElemanModel eleman;
            string sepetJson = HttpContext.Session.GetString("sepet");
            if (!string.IsNullOrWhiteSpace(sepetJson)) // byte[], int veya string olarak session'da veri kullanılabilir
            {
                sepet = JsonConvert.DeserializeObject<List<SepetElemanModel>>(sepetJson);
            }
            eleman = new SepetElemanModel()
            {
                UrunId = urunId.Value,
                KullaniciId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value),
                UrunAdi = urun.Adi,
                UrunBirimFiyati = urun.BirimFiyati ?? 0
            };
            sepet.Add(eleman);
            sepetJson = JsonConvert.SerializeObject(sepet);
            // Session'da asla kullanıcı bilgileri gibi kritik veriler tutulmamalıdır!
            HttpContext.Session.SetString("sepet", sepetJson);
            int urunAdedi = sepet.Count(s => s.UrunId == urunId);
            TempData["Sonuc"] = urunAdedi + " adet " + eleman.UrunAdi + " sepete eklendi.";
            return RedirectToAction("Index", "Urunler", new { sepetUrunId = eleman.UrunId });
        }

        public IActionResult Getir()
        {
            List<SepetElemanModel> sepet = new List<SepetElemanModel>();
            string sepetJson = HttpContext.Session.GetString("sepet");
            if (!string.IsNullOrWhiteSpace(sepetJson))
            {
                sepet = JsonConvert.DeserializeObject<List<SepetElemanModel>>(sepetJson);
            }
            return View(sepet);
        }
    }
}
