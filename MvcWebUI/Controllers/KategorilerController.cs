using AppCore.Business.Models.Results;
using Business.Models;
using Business.Services;
using Business.Services.Bases;
using Microsoft.AspNetCore.Mvc;

namespace MvcWebUI.Controllers
{
    public class KategorilerController : Controller
    {
        // Bu class'ın ihtiyacı olan objeleri class içinde new'lemek yerine dependency injection
        // (constructor injection) yapılmalı.
        //private readonly IKategoriService _kategoriService = new KategoriService();

        private readonly IKategoriService _kategoriService;

        public KategorilerController(IKategoriService kategoriService)
        {
            _kategoriService = kategoriService;
        }

        public IActionResult Index() // ~/Kategoriler/Index
        {
            List<KategoriModel> kategoriler = _kategoriService.Query().ToList();
            // ToList(), SingleOrDefault(), FirstOrDefault(), vb. methodlar Query ile
            // oluşturulan sorguyu veritabanında çalıştırır ve sonucunu bir objeye atar.

            if (kategoriler == null || kategoriler.Count == 0) // liste boş ise
                return View("Hata", "Kayıt bulunamadı.");

            //return View(); // Index.cshtml'i kullanır
            return View("KategoriListesi", kategoriler); // KategoriListesi.cshtml'i kullanır
        }

        // ~/Kategoriler/OlusturGetir
        [HttpGet] // Action Method Selector:
                  // Web server'da herhangi bir kaynak getirip client'e dönmek için kullanılır.
                  // eğer bir aksiyona herhangi bir Http attribute'u yazılmazsa default'u get'tir.
        public IActionResult OlusturGetir() // önce kullanıcıya giriş yapabileceği form sayfası getirilir
        {
            return View("OlusturHtml");
        }

        // ~/Kategoriler/OlusturGonder
        [HttpPost] // Client'ın web server'a veri göndermesi için kullanılır.
                   // Genelde HTML form'ları üzerinden method post olarak kullanılır.
                   // Eğer post (gönderme) işlemi yapılıyorsa HttpPost mutlaka yazılmalıdır.
        public IActionResult OlusturGonder(string Adi, string Aciklamasi) // kullanıcının girdiği kategori verileri gönderilir ve veritabanında oluşturulur
        {
            KategoriModel model = new KategoriModel()
            {
                Adi = Adi,
                Aciklamasi = Aciklamasi
            };
            Result result = _kategoriService.Add(model);
            if (result.IsSuccessful)
            {

                //return RedirectToAction("Index");
                return RedirectToAction(nameof(Index));

            }
            return View("Hata", result.Message); // status code: 200 (OK)
        }

        //https://httpstatuses.com/
        public IActionResult Edit(int? id) // ~/Kategoriler/Edit/5
        {
            if (id == null)
            {
                //return BadRequest(); // status code: 400
                //return BadRequest("Id gereklidir!");
                return View("Hata", "Id gereklidir!");
            }

            KategoriModel model = _kategoriService.Query().SingleOrDefault(k => k.Id == id.Value);

            if (model == null)
            {
                //return NotFound(); // status code: 404
                //return NotFound("Kayıt bulunamadı!");
                return View("Hata", "Kayıt bulunamadı!");
            }

            //return new ViewResult();
            return View(model); // status code: 200 (OK)
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(KategoriModel model)
        {
            if (ModelState.IsValid) // modelde validasyon hataları yoksa
            {
                var result = _kategoriService.Update(model);
                if (result.IsSuccessful)
                {
                    // TempData: eğer yönlendirme (redirect) işlemi varsa yönlendirilen aksiyon üzerinden dönen view'a
                    // string bir index üzerinden herhangi bir obje taşımak için kullanılır.
                    TempData["Success"] = result.Message; // Kategori başarıyla güncellendi.

                    return RedirectToAction(nameof(Index));
                }

                // eğer servis başarısız sonucu döndüyse:

                // ViewData veya ViewBag: eğer view dönülüyorsa string bir index üzerinden view'a herhangi bir obje taşımak için kullanılır.
                // ViewBag (özellik) ile ViewData (index) birbirleri yerine aynı özellik ve index adları üzerinden kullanılabilir.
                //ViewData["Error"] = result.Message; // Girdiğiniz kategori adına sahip kayıt bulunmaktadır!
                ViewBag.Error = result.Message; 
            }

            // eğer modelde validasyon hataları varsa
            return View(model);
        }

        #region IActionResult'ı implemente eden class'lar
        /*
        IActionResult
        ActionResult
        ViewResult (View())  ContentResult (Content()) EmptyResult   FileContentResult (File()) HttpResults JavaScriptResult (JavaScript())  JsonResult (Json())   RedirectResults
        */
        public ContentResult GetHtmlContent()
        {
            //return new ContentResult();
            return Content("<b><i>Content result.</i></b>", "text/html");
        }
        public ActionResult GetKategorilerXmlContent() // XML döndürme işlemleri genelde bu şekilde yapılmaz, web servisler üzerinden döndürülür!
        {
            List<KategoriModel> kategoriler = _kategoriService.Query().ToList();
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            xml += "<KategoriModels>";
            foreach (KategoriModel kategori in kategoriler)
            {
                xml += "<KategoriModel>";
                xml += "<Id>" + kategori.Id + "</Id>";
                xml += "<Adi>" + kategori.Adi + "</Adi>";
                xml += "<Aciklamasi>" + kategori.Aciklamasi + "</Aciklamasi>";
                xml += "</KategoriModel>";
            }
            xml += "</KategoriModels>";
            return Content(xml, "application/xml");
        }
        public string GetString()
        {
            return "String.";
        }
        public EmptyResult GetEmpty()
        {
            return new EmptyResult();
        }
        #endregion
    }
}
