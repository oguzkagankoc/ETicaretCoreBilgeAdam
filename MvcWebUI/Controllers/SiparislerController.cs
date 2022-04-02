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
            sepet = sepet.Where(s => s.KullaniciId == Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value)).ToList();

            // session'dan sepetin temizlenmesi:
            HttpContext.Session.Remove("sepet");

            SiparisModel siparis = new SiparisModel()
            {
                KullaniciId = sepet.FirstOrDefault().KullaniciId, // sepetteki tüm elemanlar aynı kullanıcı Id'ye sahip olduğundan ilk elemanın kullanıcı Id'sini kullanıyoruz
                UrunSiparisler = sepet.Select(s => new UrunSiparisModel()
                {
                    UrunId = s.UrunId
                }).ToList()
            };
            var result = _siparisService.Add(siparis);
            TempData["Sonuc"] = result.Message;
            return RedirectToAction(nameof(Getir));
        }

        public IActionResult Getir(SiparisFilterModel filtre = null)
        {
            var result = _siparisService.SiparisleriGetir(filtre);
            List<SiparisModel> siparisler = result.Data;
            TempData["Sonuc"] = result.Message;
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
