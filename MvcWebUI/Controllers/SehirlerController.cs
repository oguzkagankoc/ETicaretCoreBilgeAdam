using Business.Models;
using Business.Services;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MvcWebUI.Controllers
{
    public class SehirlerController : Controller
    {
        private readonly ISehirService _sehirService;
        private readonly IUlkeService _ulkeService;

        public SehirlerController(ISehirService sehirService, IUlkeService ulkeService)
        {
            _sehirService = sehirService;
            _ulkeService = ulkeService;
        }

        // GET: Sehirler
        public IActionResult Index()
        {
            return View(_sehirService.Query().ToList());
        }

        // GET: Sehirler/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            SehirModel sehir = _sehirService.Query().SingleOrDefault(s => s.Id == id.Value);
            if (sehir == null)
            {
                return View("Hata", "Kayıt bulunamadı!");
            }
            return View(sehir);
        }

        // GET: Sehirler/Create
        public IActionResult Create()
        {
            ViewData["UlkeId"] = new SelectList(_ulkeService.Query().ToList(), "Id", "Adi");
            return View();
        }

        // POST: Sehirler/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SehirModel sehir)
        {
            if (ModelState.IsValid)
            {
                var result = _sehirService.Add(sehir);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            ViewData["UlkeId"] = new SelectList(_ulkeService.Query().ToList(), "Id", "Adi", sehir.UlkeId);
            return View(sehir);
        }

        // GET: Sehirler/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            SehirModel sehir = _sehirService.Query().SingleOrDefault(s => s.Id == id.Value);
            if (sehir == null)
            {
                return View("Hata", "Kayıt bulunamadı!");
            }
            ViewData["UlkeId"] = new SelectList(_ulkeService.Query().ToList(), "Id", "Adi", sehir.UlkeId);
            return View(sehir);
        }

        // POST: Sehirler/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SehirModel sehir)
        {
            if (ModelState.IsValid)
            {
                var result = _sehirService.Update(sehir);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            ViewData["UlkeId"] = new SelectList(_ulkeService.Query().ToList(), "Id", "Adi", sehir.UlkeId);
            return View(sehir);
        }

        // GET: Sehirler/Delete/5
        public IActionResult Delete(int? id)
        {
            var result = _sehirService.Delete(id.Value);
            if (result.IsSuccessful)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
