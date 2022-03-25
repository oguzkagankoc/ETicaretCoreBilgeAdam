using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcWebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UlkelerController : Controller
    {
        private readonly IUlkeService _ulkeService;

        public UlkelerController(IUlkeService ulkeService)
        {
            _ulkeService = ulkeService;
        }

        // GET: Ulkeler
        public IActionResult Index()
        {
            return View(_ulkeService.Query().ToList());
        }

        // GET: Ulkeler/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ulkeler/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UlkeModel ulke)
        {
            if (ModelState.IsValid)
            {
                var result = _ulkeService.Add(ulke);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            return View(ulke);
        }

        // GET: Ulkeler/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            UlkeModel ulke = _ulkeService.Query().SingleOrDefault(u => u.Id == id);
            if (ulke == null)
            {
                return View("Hata", "Kayıt bulunamadı!");
            }
            return View(ulke);
        }

        // POST: Ulkeler/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UlkeModel ulke)
        {
            if (ModelState.IsValid)
            {
                var result = _ulkeService.Update(ulke);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            return View(ulke);
        }

        // GET: Ulkeler/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            var result = _ulkeService.Delete(id.Value);
            TempData["Sonuc"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
	}
}
