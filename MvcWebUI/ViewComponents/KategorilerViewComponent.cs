using Business.Models;
using Business.Services.Bases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace MvcWebUI.ViewComponents
{
    public class KategorilerViewComponent : ViewComponent
    {
        private readonly IKategoriService _kategoriService;

        public KategorilerViewComponent(IKategoriService kategoriService)
        {
            _kategoriService = kategoriService;
        }

        public ViewViewComponentResult Invoke()
        {
            List<KategoriModel> kategoriler;

            kategoriler = _kategoriService.Query().ToList();

            return View(kategoriler); // view ~/Views/Shared/Components/Kategoriler/Default.cshtml altındadır
        }
    }
}
