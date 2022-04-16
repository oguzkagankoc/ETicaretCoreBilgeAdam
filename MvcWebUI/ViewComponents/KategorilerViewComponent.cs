using Business.Models;
using Business.Services.Bases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;

namespace MvcWebUI.ViewComponents
{
    public class KategorilerViewComponent : ViewComponent
    {
        private readonly IKategoriService _kategoriService;

        public KategorilerViewComponent(IKategoriService kategoriService)
        {
            _kategoriService = kategoriService;
        }

        //public ViewViewComponentResult Invoke()
        public ViewViewComponentResult Invoke(int? kategoriId)
        {
            List<KategoriModel> kategoriler;

            // Senkron
            //kategoriler = _kategoriService.Query().ToList(); // *1

            // Asenkron
            //Task<List<KategoriModel>> task = _kategoriService.Query().ToListAsync(); // *2

            // Asenkron
            Task<List<KategoriModel>> task = _kategoriService.KategorileriGetirAsync(); // *3
            
            kategoriler = task.Result;

            ViewBag.KategoriId = kategoriId;

            return View(kategoriler); // view ~/Views/Shared/Components/Kategoriler/Default.cshtml altındadır
        }
    }
}
