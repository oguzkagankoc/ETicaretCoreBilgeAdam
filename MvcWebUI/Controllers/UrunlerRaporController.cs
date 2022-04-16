using AppCore.Business.Models.Results;
using Business.Models.Filters;
using Business.Models.Reports;
using Business.Services;
using Business.Services.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcWebUI.Models;

namespace MvcWebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UrunlerRaporController : Controller
    {
        private readonly IUrunService _urunService;
        private readonly IKategoriService _kategoriService;

        public UrunlerRaporController(IUrunService urunService, IKategoriService kategoriService)
        {
            _urunService = urunService;
            _kategoriService = kategoriService;
        }

        //public IActionResult Index()
        //public IActionResult Index(int? kategoriId)
        public async Task<IActionResult> Index(int? kategoriId) // asenkron metodlar mutlaka async Task dönmeli ve içinde çağrılan asenkron metodla birlikte await kullanılmalı!
        {
            UrunRaporFilterModel filtre = new UrunRaporFilterModel()
            {
                KategoriId = kategoriId
            };

            Result<List<UrunRaporModel>> result = await _urunService.RaporGetirAsync(filtre);
            ViewBag.Sonuc = result.Message;

            UrunlerRaporIndexViewModel viewModel = new UrunlerRaporIndexViewModel()
            {
                UrunlerRapor = result.Data,
                UrunlerFiltre = filtre,

                //KategorilerSelectList = new SelectList(await _kategoriService.KategorileriGetirAsync(), "Id", "Adi")
            };

            return View(viewModel);
        }
    }
}
