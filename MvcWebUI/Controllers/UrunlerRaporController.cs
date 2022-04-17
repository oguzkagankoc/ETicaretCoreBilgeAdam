using AppCore.Business.Models.Ordering;
using AppCore.Business.Models.Paging;
using AppCore.Business.Models.Results;
using Business.Models.Filters;
using Business.Models.Reports;
using Business.Services;
using Business.Services.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcWebUI.Models;
using MvcWebUI.Settings;

namespace MvcWebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UrunlerRaporController : Controller
    {
        private readonly IUrunService _urunService;
        private readonly IKategoriService _kategoriService; // eğer kategoriler için SelectList olşuturulacaksa gerek var, burada gerek yok
        private readonly IMagazaService _magazaService;

        public UrunlerRaporController(IUrunService urunService, IKategoriService kategoriService, IMagazaService magazaService)
        {
            _urunService = urunService;
            _kategoriService = kategoriService;
            _magazaService = magazaService;
        }

        //public IActionResult Index()
        //public IActionResult Index(int? kategoriId)
        public async Task<IActionResult> Index(int? kategoriId) // asenkron metodlar mutlaka async Task dönmeli ve içinde çağrılan asenkron metodla birlikte await kullanılmalı!
        {
            #region Filtreleme
            UrunRaporFilterModel filtre = new UrunRaporFilterModel()
            {
                KategoriId = kategoriId
            };
            #endregion

            #region Sayfalama
            PageModel sayfa = new PageModel()
            {
                RecordsPerPageCount = AppSettings.SayfaKayitSayisi
            };
            #endregion

            #region Sıralama
            OrderModel sira = new OrderModel()
            {
                //DirectionAscending = true, // özelliğin default değeri true
                Expression = "Mağaza"
            };
            List<SelectListItem> siraSutunBasliklariSelectListItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Value = "Mağaza",
                    Text = "Mağaza"
                },
                new SelectListItem()
                {
                    Value = "Kategori",
                    Text = "Kategori"
                },
                new SelectListItem()
                {
                    Value = "Ürün",
                    Text = "Ürün"
                },
                new SelectListItem()
                {
                    Value = "Birim Fiyatı",
                    Text = "Birim Fiyatı"
                },
                new SelectListItem()
                {
                    Value = "Stok Miktarı",
                    Text = "Stok Miktarı"
                },
                new SelectListItem()
                {
                    Value = "Son Kullanma Tarihi",
                    Text = "Son Kullanma Tarihi"
                }
            };
            List<SelectListItem> siraYonSelectListItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Value = "True",
                    Text = "Artan"
                },
                new SelectListItem()
                {
                    Value = "False",
                    Text = "Azalan"
                }
            };
            #endregion

            Result<List<UrunRaporModel>> result = await _urunService.RaporGetirAsync(filtre, sayfa, sira);
            ViewBag.Sonuc = result.Message;

            UrunlerRaporIndexViewModel viewModel = new UrunlerRaporIndexViewModel()
            {
                UrunlerRapor = result.Data,
                UrunlerFiltre = filtre,

                //KategorilerSelectList = new SelectList(await _kategoriService.KategorileriGetirAsync(), "Id", "Adi"),

                MagazalarMultiSelectList = new MultiSelectList(_magazaService.Query().ToList(), "Id", "Adi"),

                // Sayfalama
                SayfalarSelectList = new SelectList(sayfa.Pages, "Value", "Text"),

                // Sıralama
                SiraSutunBasliklariSelectList = new SelectList(siraSutunBasliklariSelectListItems, "Value", "Text"),
                SiraYonSelectList = new SelectList(siraYonSelectListItems, "Value", "Text")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UrunlerRaporIndexViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                #region Sayfalama
                viewModel.Sayfa.RecordsPerPageCount = AppSettings.SayfaKayitSayisi;
                #endregion

                #region Sıralama
                List<SelectListItem> siraSutunBasliklariSelectListItems = new List<SelectListItem>()
                {
                    new SelectListItem()
                    {
                        Value = "Mağaza",
                        Text = "Mağaza"
                    },
                    new SelectListItem()
                    {
                        Value = "Kategori",
                        Text = "Kategori"
                    },
                    new SelectListItem()
                    {
                        Value = "Ürün",
                        Text = "Ürün"
                    },
                    new SelectListItem()
                    {
                        Value = "Birim Fiyatı",
                        Text = "Birim Fiyatı"
                    },
                    new SelectListItem()
                    {
                        Value = "Stok Miktarı",
                        Text = "Stok Miktarı"
                    },
                    new SelectListItem()
                    {
                        Value = "Son Kullanma Tarihi",
                        Text = "Son Kullanma Tarihi"
                    }
                };
                List<SelectListItem> siraYonSelectListItems = new List<SelectListItem>()
                {
                    new SelectListItem()
                    {
                        Value = "True",
                        Text = "Artan"
                    },
                    new SelectListItem()
                    {
                        Value = "False",
                        Text = "Azalan"
                    }
                };
                #endregion

                Result<List<UrunRaporModel>> result = await _urunService.RaporGetirAsync(viewModel.UrunlerFiltre, viewModel.Sayfa, viewModel.Sira);
                ViewBag.Sonuc = result.Message;
                viewModel.UrunlerRapor = result.Data;

                // partial view'da kullanılan sayfa ve sıra tekrar doldurulmalıdır ki partial view bunlar üzerinden güncellenebilsin
                // Sayfalama
                viewModel.SayfalarSelectList = new SelectList(viewModel.Sayfa.Pages, "Value", "Text", viewModel.Sayfa.PageNumber);

                // Sıralama
                viewModel.SiraSutunBasliklariSelectList = new SelectList(siraSutunBasliklariSelectListItems, "Value", "Text", viewModel.Sira.Expression);
                viewModel.SiraYonSelectList = new SelectList(siraYonSelectListItems, "Value", "Text", viewModel.Sira.DirectionAscending);
            }
            return PartialView("_UrunlerRapor", viewModel);
        }
    }
}
