using Business.Services;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MvcWebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KullanicilarController : Controller
    {
        private readonly ETicaretContext _context;

        private readonly IKullaniciService _kullaniciService;

        public KullanicilarController(IKullaniciService kullaniciService)
        {
            _kullaniciService = kullaniciService;
        }

        // GET: Kullanicilar
        public IActionResult Index()
        {
            var result = _kullaniciService.KullanicilariGetir();
            if (!result.IsSuccessful)
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
            ViewData["RolId"] = new SelectList(_context.Roller, "Id", "Adi");
            return View();
        }

        // POST: Kullanicilar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kullanici);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.Roller, "Id", "Adi", kullanici.RolId);
            return View(kullanici);
        }

        // GET: Kullanicilar/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kullanici = _context.Kullanicilar.Find(id);
            if (kullanici == null)
            {
                return NotFound();
            }
            ViewData["RolId"] = new SelectList(_context.Roller, "Id", "Adi", kullanici.RolId);
            return View(kullanici);
        }

        // POST: Kullanicilar/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                _context.Update(kullanici);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.Roller, "Id", "Adi", kullanici.RolId);
            return View(kullanici);
        }

        // GET: Kullanicilar/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kullanici = _context.Kullanicilar
                .Include(k => k.Rol)
                .SingleOrDefault(m => m.Id == id);
            if (kullanici == null)
            {
                return NotFound();
            }

            return View(kullanici);
        }

        // POST: Kullanicilar/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var kullanici = _context.Kullanicilar.Find(id);
            _context.Kullanicilar.Remove(kullanici);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
	}
}
