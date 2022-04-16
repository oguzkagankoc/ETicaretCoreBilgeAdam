using Business.Models.Filters;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcWebUI.Models;

namespace MvcWebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UrunlerRaporController : Controller
    {
        private readonly IUrunService _urunService;

        public UrunlerRaporController(IUrunService urunService)
        {
            _urunService = urunService;
        }

        //public IActionResult Index()
        public IActionResult Index(int? kategoriId)
        {
            UrunRaporFilterModel filtre = new UrunRaporFilterModel()
            {
                KategoriId = kategoriId
            };

            var result = _urunService.RaporGetir(filtre);
            ViewBag.Sonuc = result.Message;

            UrunlerRaporIndexViewModel viewModel = new UrunlerRaporIndexViewModel()
            {
                UrunlerRapor = result.Data,
                UrunlerFiltre = filtre
            };

            return View(viewModel);
        }
    }
}
