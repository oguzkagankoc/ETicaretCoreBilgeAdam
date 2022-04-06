using System.Security.Claims;
using Business.Models;
using Business.Models.Filters;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcWebUI.Models;
using Newtonsoft.Json;

namespace MvcWebUI.Controllers
{
    [Authorize]
    public class SiparislerController : Controller
    {
        private readonly ISiparisService _siparisService;

        public SiparislerController(ISiparisService siparisService)
        {
            _siparisService = siparisService;
        }

        public IActionResult Al()
        {
            string sepetJson = HttpContext.Session.GetString("sepet");
            if (string.IsNullOrWhiteSpace(sepetJson))
                return RedirectToAction("Index", "Urunler");
            List<SepetElemanModel> sepet = JsonConvert.DeserializeObject<List<SepetElemanModel>>(sepetJson);

            // session'dan alınan sepetin sisteme giriş yapan kullanıcı Id'sine göre filtrelenmesi:
            List<SepetElemanModel> kullaniciSepeti = sepet.Where(s => s.KullaniciId == Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value)).ToList();

            SiparisModel siparis = new SiparisModel()
            {
                KullaniciId = kullaniciSepeti.FirstOrDefault().KullaniciId, // kullanıcı sepetindeki tüm elemanlar aynı kullanıcı Id'ye sahip olduğundan ilk elemanın kullanıcı Id'sini kullanıyoruz
                UrunSiparisler = kullaniciSepeti.Select(s => new UrunSiparisModel()
                {
                    UrunId = s.UrunId
                }).ToList()
            };
            
            // session'dan kullanıcıya ait sepetin temizlenmesi:
            foreach (SepetElemanModel eleman in kullaniciSepeti)
            {
                sepet.Remove(eleman);
            }
            sepetJson = JsonConvert.SerializeObject(sepet);
            HttpContext.Session.SetString("sepet", sepetJson);

            var result = _siparisService.Add(siparis);
            TempData["Sonuc"] = result.Message;
            return RedirectToAction(nameof(Getir));
        }

        public IActionResult Getir(SiparisFilterModel filtre = null)
        {
            if (User.IsInRole("Kullanıcı")) // eğer Kullanıcı rolündeyse kullanıcının sadece kendi siparişlerini görebilmesi için yeni bir filtre oluştur ve KullaniciId'yi User'dan ata
            {
                if (filtre == null) // filtre null is new'le
                    filtre = new SiparisFilterModel();
                filtre.KullaniciId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value); 
            }

            var result = _siparisService.SiparisleriGetir(filtre);
            List<SiparisModel> siparisler = result.Data;
            ViewBag.Sonuc = result.Message;
            SiparislerGetirViewModel viewModel = new SiparislerGetirViewModel()
            {
                Siparisler = siparisler,
                Filtre = filtre ?? new SiparisFilterModel()
            };
            return View(viewModel);
        }

        public IActionResult IptalEt(int? id)
        {
            if (id == null)
                return View("Hata", "Id gereklidir!");
            var result = _siparisService.Delete(id.Value);
            TempData["Sonuc"] = result.Message;
            return RedirectToAction(nameof(Getir));
        }

        [Authorize(Roles = "Admin")] // alınan siparişleri sadece Admin rolündekiler tamamlayabilir
        public IActionResult Tamamla(int? id)
        {
            if (id == null)
                return View("Hata", "Id gereklidir!");
            var result = _siparisService.Update(new SiparisModel() {Id = id.Value});
            TempData["Sonuc"] = result.Message;
            return RedirectToAction(nameof(Getir));
        }
    }
}
