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
            return View("KategoriListesi", kategoriler); // Kategoriler.cshtml'i kullanır
        }
    }
}
