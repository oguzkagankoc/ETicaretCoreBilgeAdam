using Business.Models;
using Business.Services;
using Business.Services.Bases;
using Microsoft.AspNetCore.Mvc;

namespace MvcWebUI.Controllers
{
    public class KategoriController : Controller
    {
        // Bu class'ın ihtiyacı olan objeleri class içinde new'lemek yerine dependency injection
        // (constructor injection) yapılmalı.
        //private readonly IKategoriService _kategoriService = new KategoriService();

        private readonly IKategoriService _kategoriService;

        public KategoriController(IKategoriService kategoriService)
        {
            _kategoriService = kategoriService;
        }

        public IActionResult Index() // ~/Kategori/Index
        {
            List<KategoriModel> kategoriler = _kategoriService.Query().ToList();
            // ToList(), SingleOrDefault(), FirstOrDefault(), vb. methodlar Query ile
            // oluşturulan sorguyu veritabanında çalıştırır ve sonucunu bir objeye atar.

            //return View(); // Index.cshtml'i kullanır
            return View("Kategoriler", kategoriler); // Kategoriler.cshtml'i kullanır
        }
    }
}
