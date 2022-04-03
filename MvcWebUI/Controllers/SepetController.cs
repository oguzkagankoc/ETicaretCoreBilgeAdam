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
            sepetJson = JsonConvert.SerializeObject(sepet);
            // Session'da asla kullanıcı bilgileri gibi kritik veriler tutulmamalıdır!
            HttpContext.Session.SetString("sepet", sepetJson);
            int urunAdedi = sepet.Count(s => s.UrunId == urunId);
            TempData["Sonuc"] = urunAdedi + " adet " + eleman.UrunAdi + " sepete eklendi.";
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

            return View(sepet);
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
