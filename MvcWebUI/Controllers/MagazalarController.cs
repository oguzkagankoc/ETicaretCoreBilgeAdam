using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Create(MagazaModel magaza)
        {
            if (ModelState.IsValid)
            {
                var result = _magazaService.Add(magaza);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            return View(magaza);
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
