using Business.Models;
using Business.Services;
using Business.Services.Bases;
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
        private readonly IKategoriService _kategoriService;

        public UrunlerController(IUrunService urunService, IKategoriService kategoriService)
        {
            _urunService = urunService;
            _kategoriService = kategoriService;
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
        //public IActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var urun = _context.Urunler
        //        .Include(u => u.Kategori)
        //        .SingleOrDefault(m => m.Id == id);
        //    if (urun == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(urun);
        //}
        public IActionResult Details(int? id)
        {
            if (id == null)
                return View("Hata", "Id gereklidir!");
            UrunModel model = _urunService.Query().SingleOrDefault(u => u.Id == id.Value);
            if (model == null)
                return View("Hata", "Kayıt bulunamadı!");
            return View(model);
        }

        // GET: Urunler/Create
        //public IActionResult Create()
        //{
        //    ViewData["KategoriId"] = new SelectList(_context.Kategoriler, "Id", "Adi");
        //    return View();
        //}
        public IActionResult Create()
        {
            List<KategoriModel> kategoriler = _kategoriService.Query().ToList();
            ViewBag.KategoriId = new SelectList(kategoriler, "Id", "Adi");
            return View();
        }

        // POST: Urunler/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult Create(Urun urun)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(urun);
        //        _context.SaveChanges();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["KategoriId"] = new SelectList(_context.Kategoriler, "Id", "Adi", urun.KategoriId);
        //    return View(urun);
        //}
        public IActionResult Create(UrunModel urun)
        {
            if (ModelState.IsValid)
            {
                var result = _urunService.Add(urun);
                if (result.IsSuccessful)
                {
                    TempData["Success"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.KategoriId = new SelectList(_kategoriService.Query().ToList(), "Id", "Adi", urun.KategoriId);
            return View(urun);
        }

        // GET: Urunler/Edit/5
        //public IActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var urun = _context.Urunler.Find(id);
        //    if (urun == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["KategoriId"] = new SelectList(_context.Kategoriler, "Id", "Adi", urun.KategoriId);
        //    return View(urun);
        //}
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return View("Hata", "Id gereklidir!");
            UrunModel model = _urunService.Query().SingleOrDefault(u => u.Id == id.Value);
            if (model == null)
                return View("Hata", "Kayıt bulunamadı!");
            ViewBag.KategoriId = new SelectList(_kategoriService.Query().ToList(), "Id", "Adi", model.KategoriId);
            return View(model);
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
