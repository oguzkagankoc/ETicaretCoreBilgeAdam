using Business.Models;
using Business.Services;
using DataAccess.Contexts;
using DataAccess.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcWebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KullanicilarController : Controller
    {
        private readonly ETicaretContext _context;

        private readonly IKullaniciService _kullaniciService;
        private readonly IRolService _rolService;
        private readonly IUlkeService _ulkeService;
        private readonly ISehirService _sehirService;

        public KullanicilarController(IKullaniciService kullaniciService, IRolService rolService, IUlkeService ulkeService, ISehirService sehirService)
        {
            _kullaniciService = kullaniciService;
            _rolService = rolService;
            _ulkeService = ulkeService;
            _sehirService = sehirService;
        }

        // GET: Kullanicilar
        public IActionResult Index()
        {
            var result = _kullaniciService.KullanicilariGetir();
            ViewBag.Sonuc = result.Message;
            return View(result.Data);
        }

        // GET: Kullanicilar/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            var result = _kullaniciService.KullaniciGetir(id.Value);
            if (!result.IsSuccessful)
            {
                ViewBag.Sonuc = result.Message;
            }
            return View(result.Data);
        }

        // GET: Kullanicilar/Create
        public IActionResult Create()
        {
            var rolSonuc = _rolService.RolleriGetir();
            var ulkeSonuc = _ulkeService.UlkeleriGetir();
            ViewData["RolId"] = new SelectList(rolSonuc.Data, "Id", "Adi");
            ViewData["UlkeId"] = new SelectList(ulkeSonuc.Data, "Id", "Adi");
            KullaniciModel model = new KullaniciModel()
            {
                AktifMi = true,
                KullaniciDetayi = new KullaniciDetayiModel()
                {
                    Cinsiyet = Cinsiyet.Kadın
                }
            };
            return View(model);
        }

        // POST: Kullanicilar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(KullaniciModel kullanici)
        {
            ModelState.Remove(nameof(kullanici.Id));
            if (ModelState.IsValid)
            {
                var result = _kullaniciService.Add(kullanici);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            var rolSonuc = _rolService.RolleriGetir();
            var ulkeSonuc = _ulkeService.UlkeleriGetir();
            var sehirSonuc = _sehirService.SehirleriGetir(kullanici.KullaniciDetayi.UlkeId ?? -1);
            ViewData["RolId"] = new SelectList(rolSonuc.Data, "Id", "Adi", kullanici.RolId);
            ViewData["UlkeId"] = new SelectList(ulkeSonuc.Data, "Id", "Adi", kullanici.KullaniciDetayi.UlkeId ?? -1);
            ViewData["SehirId"] = new SelectList(sehirSonuc.Data, "Id", "Adi", kullanici.KullaniciDetayi.SehirId ?? -1);
            return View(kullanici);
        }

        // GET: Kullanicilar/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            var result = _kullaniciService.KullaniciGetir(id.Value);
            if (!result.IsSuccessful)
            {
                return View("Hata", result.Message);
            }
            var rolSonuc = _rolService.RolleriGetir();
            var ulkeSonuc = _ulkeService.UlkeleriGetir();
            var sehirSonuc = _sehirService.SehirleriGetir(result.Data.KullaniciDetayi.UlkeId ?? -1);
            ViewData["RolId"] = new SelectList(rolSonuc.Data, "Id", "Adi", result.Data.RolId);
            ViewData["UlkeId"] = new SelectList(ulkeSonuc.Data, "Id", "Adi", result.Data.KullaniciDetayi.UlkeId ?? -1);
            ViewData["SehirId"] = new SelectList(sehirSonuc.Data, "Id", "Adi", result.Data.KullaniciDetayi.SehirId ?? -1);
            return View(result.Data);
        }

        // POST: Kullanicilar/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(KullaniciModel kullanici)
        {
            if (ModelState.IsValid)
            {
                var result = _kullaniciService.Update(kullanici);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            var rolSonuc = _rolService.RolleriGetir();
            var ulkeSonuc = _ulkeService.UlkeleriGetir();
            var sehirSonuc = _sehirService.SehirleriGetir(kullanici.KullaniciDetayi.UlkeId ?? -1);
            ViewData["RolId"] = new SelectList(rolSonuc.Data, "Id", "Adi", kullanici.RolId);
            ViewData["UlkeId"] = new SelectList(ulkeSonuc.Data, "Id", "Adi", kullanici.KullaniciDetayi.UlkeId ?? -1);
            ViewData["SehirId"] = new SelectList(sehirSonuc.Data, "Id", "Adi", kullanici.KullaniciDetayi.SehirId ?? -1);
            return View(kullanici);
        }

        // GET: Kullanicilar/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            var result = _kullaniciService.Delete(id.Value);
            TempData["Sonuc"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
