using Business.Models;
using Business.Services;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MvcWebUI.Controllers
{
    public class UrunlerController : Controller
    {
        private readonly ETicaretContext _context;

        //public UrunlerController(ETicaretContext context)
        //{
        //    _context = context;
        //}

        private readonly IUrunService _urunService;

        public UrunlerController(IUrunService urunService)
        {
            _urunService = urunService;
        }

        // GET: Urunler
        //public IActionResult Index()
        //{
        //    var eTicaretContext = _context.Urunler.Include(u => u.Kategori);
        //    return View(eTicaretContext.ToList());
        //}
        public IActionResult Index()
        {
            List<UrunModel> model = _urunService.Query().ToList();
            return View(model);
        }

        // GET: Urunler/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = _context.Urunler
                .Include(u => u.Kategori)
                .SingleOrDefault(m => m.Id == id);
            if (urun == null)
            {
                return NotFound();
            }

            return View(urun);
        }

        // GET: Urunler/Create
        public IActionResult Create()
        {
            ViewData["KategoriId"] = new SelectList(_context.Kategoriler, "Id", "Adi");
            return View();
        }

        // POST: Urunler/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Urun urun)
        {
            if (ModelState.IsValid)
            {
                _context.Add(urun);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KategoriId"] = new SelectList(_context.Kategoriler, "Id", "Adi", urun.KategoriId);
            return View(urun);
        }

        // GET: Urunler/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = _context.Urunler.Find(id);
            if (urun == null)
            {
                return NotFound();
            }
            ViewData["KategoriId"] = new SelectList(_context.Kategoriler, "Id", "Adi", urun.KategoriId);
            return View(urun);
        }

        // POST: Urunler/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Urun urun)
        {
            if (ModelState.IsValid)
            {
                _context.Update(urun);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KategoriId"] = new SelectList(_context.Kategoriler, "Id", "Adi", urun.KategoriId);
            return View(urun);
        }

        // GET: Urunler/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = _context.Urunler
                .Include(u => u.Kategori)
                .SingleOrDefault(m => m.Id == id);
            if (urun == null)
            {
                return NotFound();
            }

            return View(urun);
        }

        // POST: Urunler/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var urun = _context.Urunler.Find(id);
            _context.Urunler.Remove(urun);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
	}
}
