using AppCore.Business.Models.Results;
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

        private readonly IMagazaService _magazaService;

        public UrunlerController(IUrunService urunService, IKategoriService kategoriService, IMagazaService magazaService)
        {
            _urunService = urunService;
            _kategoriService = kategoriService;

            _magazaService = magazaService;
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

            ViewBag.Magazalar = new MultiSelectList(_magazaService.Query().ToList(), "Id", "Adi");

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
                // İmaj dosya adları olarak ürünün Id'si ile yüklenen imajın dosya uzantısını kullanacağımızdan Id veritabanında ekleme işlemi sonucunda otomatik oluşacaktır, ancak dosya uzantısını servise göndermek ve dolayısıyla da tabloya kaydetmek zorundayız.
                ImajDosyaUzantisiniGuncelle(urun, imaj); // urun parametresi referans tip olduğu için method içinde güncellenecek ve sonucu urun üzerinden dışarıya yansıyacaktır

                var result = _urunService.Add(urun);
                if (result.IsSuccessful)
                {
                    bool? imajKaydetSonuc = ImajKaydet(urun, imaj);
                    if (imajKaydetSonuc == false) // imaj uzantı ve boyut validasyonlarını geçememiş demektir
                    {
                        result.Message += $" İmaj yüklenemedi! Yüklenen imaj uzantıları {AppSettings.ImajDosyaUzantilari} uzantılarından biri ve boyutu maksimum {AppSettings.ImajMaksimumDosyaBoyutu} mega byte olmalıdır!";
                    }
                    TempData["Success"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.KategoriId = new SelectList(_kategoriService.Query().ToList(), "Id", "Adi", urun.KategoriId);

            ViewBag.Magazalar = new MultiSelectList(_magazaService.Query().ToList(), "Id", "Adi", urun.MagazaIdleri);

            return View(urun);
        }

        private bool? ImajKaydet(UrunModel model, IFormFile yuklenenImaj, bool uzerineYazilsinMi = false)
        {
            #region Dosya validasyonu
            bool? sonuc = null; // flag, sonuc'un null dönmesi demek kullanıcının dosya seçip yüklememesi demek
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
                // Sanal dosya yolu (virtual path): ~/wwwroot/dosyalar/urunler/asusrog.jpg
                yuklenenDosyaAdi = model.Id + yuklenenDosyaUzantisi; // 1.jpg
                string dosyaYolu = Path.Combine("wwwroot", "dosyalar", "urunler", yuklenenDosyaAdi);
                // Fiziksel dosya yolu (absolute path): C:\çağıl\ETicaretCoreBilgeAdam\MvcWebUI\wwwroot\dosyalar\urunler\asusrog.jpg

                using (FileStream fileStream = new FileStream(dosyaYolu, uzerineYazilsinMi ? FileMode.Create : FileMode.CreateNew)) // CreateNew: eğer aynı isimde dosya varsa hata verir, Create ise üzerine yazar
                {
                    yuklenenImaj.CopyTo(fileStream);
                }
            }
            #endregion

            #region Eğer varsa aynı ad ve farklı uzantıya sahip dosyanın silinmesi
            if (sonuc == true)
            {
                ImajSil(model, yuklenenDosyaUzantisi);
            }
            #endregion
            return sonuc;
        }

        private void ImajSil(UrunModel model, string yuklenenDosyaUzantisi = null) // eğer güncelleme işlemi ise yüklenen dosya uzantısı dolu geleceğinden dosya uzantısı değiştiyse eski dosya uzantısına sahip dosya silinir, eğer silme işlemi ise yüklenen dosya uzantısı null olacağından direkt mevcut dosya uzantısına sahip dosya silinir
        {
            string eskiImajDosyaUzantisi = string.IsNullOrWhiteSpace(model.ImajDosyaYoluDisplay) ? null : Path.GetExtension(model.ImajDosyaYoluDisplay); // view'da gizli olarak tuttuğumuz (ImajDosyaYoluDisplay) üzerinden kullanıcının daha önce yüklemiş olduğu dosyanın uzantısı
            if (string.IsNullOrWhiteSpace(yuklenenDosyaUzantisi) || (!string.IsNullOrWhiteSpace(eskiImajDosyaUzantisi) && eskiImajDosyaUzantisi != yuklenenDosyaUzantisi)) // eğer dosya yüklenmemişse (silme işlemi) veya eski imajın dosya uzantısı yüklenen dosya uzantısından farklıysa (güncelleme işlemi) dosya sunucudan silinir
            {
                string dosyaYolu = Path.Combine("wwwroot", "dosyalar", "urunler", model.Id + eskiImajDosyaUzantisi);
                if (System.IO.File.Exists(dosyaYolu))
                    System.IO.File.Delete(dosyaYolu);
            }
        }

        private void ImajDosyaUzantisiniGuncelle(UrunModel model, IFormFile imaj)
        {
            string eskiImajDosyaUzantisi = string.IsNullOrWhiteSpace(model.ImajDosyaYoluDisplay) ? null : Path.GetExtension(model.ImajDosyaYoluDisplay); // view'da gizli olarak tuttuğumuz (ImajDosyaYoluDisplay) üzerinden kullanıcının daha önce yüklemiş olduğu dosyanın uzantısı
            model.ImajDosyaUzantisi = imaj != null && imaj.Length > 0 ? Path.GetExtension(imaj.FileName) : eskiImajDosyaUzantisi; // kullanıcının yeni yüklediği dosyanın uzantısı
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

            ViewBag.Magazalar = new MultiSelectList(_magazaService.Query().ToList(), "Id", "Adi", model.MagazaIdleri);

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
        //public IActionResult Edit(UrunModel model) // form üzerinden gönderilen imaj dosyasını kullanabilmek için değiştirildi (form'daki input type file HTML elemanının name attribute'una dikkat!)
        public IActionResult Edit(UrunModel model, IFormFile imaj)
        {
            if (ModelState.IsValid)
            {
                ImajDosyaUzantisiniGuncelle(model, imaj);

                var result = _urunService.Update(model);
                if (result.IsSuccessful)
                {
                    bool? imajKaydetSonuc = ImajKaydet(model, imaj, true);
                    if (imajKaydetSonuc == false) // imaj uzantı ve boyut validasyonlarını geçememiş demektir
                    {
                        result.Message += $" İmaj yüklenemedi! Yüklenen imaj uzantıları {AppSettings.ImajDosyaUzantilari} uzantılarından biri ve boyutu maksimum {AppSettings.ImajMaksimumDosyaBoyutu} mega byte olmalıdır!";
                    }
                    TempData["Success"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.KategoriId = new SelectList(_kategoriService.Query().ToList(), "Id", "Adi", model.KategoriId);

            ViewBag.Magazalar = new MultiSelectList(_magazaService.Query().ToList(), "Id", "Adi", model.MagazaIdleri);

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
        //public IActionResult DeleteConfirmed(int id) // dosya silme işlemi için değiştirildi
        public IActionResult DeleteConfirmed(int id, string imajDosyaYoluDisplay)
        {
            var result = _urunService.Delete(id);
            //if (result.IsSuccessful) // Delete methodu her zaman başarılı sonucunu döneceğinden if'le kontrole gerek yok
            //{

            UrunModel model = new UrunModel()
            {
                Id = id,
                ImajDosyaYoluDisplay = imajDosyaYoluDisplay
            };
            ImajSil(model);

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
            //}
        }
    }
}
