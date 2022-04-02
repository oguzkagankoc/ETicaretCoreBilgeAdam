using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

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
    }
}
