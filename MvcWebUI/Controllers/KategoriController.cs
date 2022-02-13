using Microsoft.AspNetCore.Mvc;

namespace MvcWebUI.Controllers
{
    public class KategoriController : Controller
    {
        private readonly IKategoriService _kategoriService;

        public IActionResult Index()
        {
            return View();
        }
    }
}
