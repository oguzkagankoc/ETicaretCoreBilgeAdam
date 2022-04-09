using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcWebUI.Settings;

namespace MvcWebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MagazalarController : Controller
    {
        private readonly IMagazaService _magazaService;

        public MagazalarController(IMagazaService magazaService)
        {
            _magazaService = magazaService;
        }

        // GET: Magazalar
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_magazaService.Query().ToList());
        }

        // GET: Magazalar/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            var magaza = _magazaService.Query().SingleOrDefault(m => m.Id == id);
            if (magaza == null)
            {
                return View("Hata", "Kayıt bulunamadı!");
            }
            return View(magaza);
        }

        // GET: Magazalar/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Magazalar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MagazaModel magaza, IFormFile imaj)
        {
            if (ModelState.IsValid)
            {
                bool? imajKaydetSonuc = ImajDosyasiniGuncelle(magaza, imaj);
                if (imajKaydetSonuc == false) // imaj uzantı ve boyut validasyonlarını geçememiş demektir
                {
                    ModelState.AddModelError("", $" İmaj yüklenemedi! Yüklenen imaj uzantıları {AppSettings.ImajDosyaUzantilari} uzantılarından biri ve boyutu maksimum {AppSettings.ImajMaksimumDosyaBoyutu} mega byte olmalıdır!");
                    return View(magaza);
                }

                var result = _magazaService.Add(magaza);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            return View(magaza);
        }

        private bool? ImajDosyasiniGuncelle(MagazaModel model, IFormFile yuklenenImaj)
        {
            #region Dosya validasyonu
            bool? sonuc = null;
            string yuklenenDosyaAdi = null, yuklenenDosyaUzantisi = null;
            if (yuklenenImaj != null && yuklenenImaj.Length > 0) // yüklenen imaj verisi varsa
            {
                sonuc = false; // validasyonu geçemedi ilk değer ataması
                yuklenenDosyaAdi = yuklenenImaj.FileName; // asusrog.jpg
                yuklenenDosyaUzantisi = Path.GetExtension(yuklenenDosyaAdi); // .jpg
                string[] imajDosyaUzantilari = AppSettings.ImajDosyaUzantilari.Split(',');
                foreach (string imajDosyaUzantisi in imajDosyaUzantilari)
                {
                    if (yuklenenDosyaUzantisi.ToLower() == imajDosyaUzantisi.ToLower().Trim())
                    {
                        sonuc = true; // imaj uzantısı validasyonunu geçti
                        break;
                    }
                }
                if (sonuc == true) // eğer imaj uzantısı validasyonunu geçtiyse imaj boyutunu valide edelim
                {
                    // 1 byte = 8 bits
                    // 1 kilobyte = 1024 bytes
                    // 1 megabyte = 1024 kilobytes = 1024 * 1024 bytes = 1.048.576 bytes
                    double imajDosyaBoyutu = AppSettings.ImajMaksimumDosyaBoyutu * Math.Pow(1024, 2); // bytes
                    if (yuklenenImaj.Length > imajDosyaBoyutu)
                        sonuc = false; // imaj boyutu validasyonunu geçemedi
                }
            }
            #endregion

            #region Dosyanın kaydedilmesi
            if (sonuc == true)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    yuklenenImaj.CopyTo(memoryStream);
                    model.Imaj = memoryStream.ToArray();
                    model.ImajDosyaUzantisi = yuklenenDosyaUzantisi;
                }
            }
            #endregion

            return sonuc;
        }

        // GET: Magazalar/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            var magaza = _magazaService.Query().SingleOrDefault(m => m.Id == id);
            if (magaza == null)
            {
                return View("Hata", "Kayıt bulunamadı!");
            }
            return View(magaza);
        }

        // POST: Magazalar/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MagazaModel magaza)
        {
            if (ModelState.IsValid)
            {
                var result = _magazaService.Update(magaza);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            return View(magaza);
        }

        // GET: Magazalar/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            _magazaService.Delete(id.Value);
            return RedirectToAction(nameof(Index));
        }
    }
}
