using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Index()
        {
            var result = _urunService.RaporGetir();
            ViewBag.Sonuc = result.Message;
            return View(result.Data);
        }
    }
}
