using Business.Models;
using Business.Services;
using Business.Services.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcWebUI.Settings;

namespace MvcWebUI.Controllers
{
    [Authorize]
    public class UrunlerController : Controller
    {
        //private readonly ETicaretContext _context;

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
        [AllowAnonymous] // controller üzerindeki Authorize attribute'unu devre dışı bırakarak herkesin aksiyona ulşamasını sağlar.
        // Sepete ekleme işlemi sonucunda eklenen ürün ID'si üzerinden mesaj gösterilmesi için değiştirildi
        //public IActionResult Index()
        //{
        //    List<UrunModel> model = _urunService.Query().ToList();
        //    return View(model);
        //}
        public IActionResult Index(int? sepetUrunId)
        {
            List<UrunModel> urunler = _urunService.Query().ToList();
            if (sepetUrunId.HasValue)
            {
                UrunModel urun = urunler.SingleOrDefault(u => u.Id == sepetUrunId.Value);
                urun.SepeteEklendiMi = true;
            }
            return View(urunler);
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
        //[Authorize(Roles = "Admin,Kullanıcı")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            List<KategoriModel> kategoriler = _kategoriService.Query().ToList();
            ViewBag.KategoriId = new SelectList(kategoriler, "Id", "Adi");

            // model üzerinden bazı ilk verilerin atanması
            //return View();
            UrunModel model = new UrunModel()
            {
                BirimFiyati = 0,
                StokMiktari = 0
            };
            return View(model);
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
        [Authorize(Roles = "Admin")]
        //public IActionResult Create(UrunModel urun) // form üzerinden gönderilen imaj dosyasını kullanabilmek için değiştirildi (form'daki input type file HTML elemanının name attribute'una dikkat!)
        public IActionResult Create(UrunModel urun, IFormFile imaj)
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

        private bool ImajKaydedilecekMi(IFormFile imaj)
        {
            bool sonuc = true; // flag
            string dosyaAdi = null, dosyaUzantisi = null;
            if (imaj != null && imaj.Length > 0) // imaj verisi varsa
            {
                dosyaAdi = imaj.FileName; // asusrog.jpg
                dosyaUzantisi = Path.GetExtension(dosyaAdi); // .jpg
                string[] imajDosyaUzantilari = AppSettings.ImajDosyaUzantilari.Split(',');
                foreach (string imajDosyaUzantisi in imajDosyaUzantilari)
                {
                   if (dosyaUzantisi.ToLower() != imajDosyaUzantisi.ToLower().Trim())
                    {
                        sonuc = false;
                        break;
                    }
                }
            }
            else // imaj verisi yoksa
            {
                sonuc = false;
            }
            return sonuc;
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
        [Authorize(Roles = "Admin")]
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
        //public IActionResult Edit(Urun urun)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Update(urun);
        //        _context.SaveChanges();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["KategoriId"] = new SelectList(_context.Kategoriler, "Id", "Adi", urun.KategoriId);
        //    return View(urun);
        //}
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(UrunModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _urunService.Update(model);
                if (result.IsSuccessful)
                {
                    TempData["Success"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.KategoriId = new SelectList(_kategoriService.Query().ToList(), "Id", "Adi", model.KategoriId);
            return View(model);
        }

        // GET: Urunler/Delete/5
        //public IActionResult Delete(int? id)
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
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return View("Hata", "Id gereklidir!");
            UrunModel model = _urunService.Query().SingleOrDefault(u => u.Id == id.Value);
            if (model == null)
                return View("Hata", "Kayıt bulunamadı!");
            return View(model);
        }

        // POST: Urunler/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //public IActionResult DeleteConfirmed(int id)
        //{
        //    var urun = _context.Urunler.Find(id);
        //    _context.Urunler.Remove(urun);
        //    _context.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //}
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var result = _urunService.Delete(id);
            //if (result.IsSuccessful) // Delete methodu her zaman başarılı sonucunu döneceğinden if'le kontrole gerek yok
            //{
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
            //}
        }
    }
}
