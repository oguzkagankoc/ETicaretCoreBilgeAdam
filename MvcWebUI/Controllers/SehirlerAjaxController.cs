using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace MvcWebUI.Controllers
{
    // Şehirler AJAX işlemi için route tanımı 2. yöntem, 1. yöntem Program.cs altındadır.
    //[Route("Iller")] // ~/Iller
    [Route("[controller]")] // ~/SehirlerAjax
    public class SehirlerAjaxController : Controller
    {
        private readonly ISehirService _sehirService;

        public SehirlerAjaxController(ISehirService sehirService)
        {
            _sehirService = sehirService;
        }

        //[Route("{ulkeId?}")] // ~/Iller/1
        [Route("SehirlerGet/{ulkeId?}")] // ~/SehirlerAjax/SehirlerGet/1
        public IActionResult SehirlerGet(int? ulkeId)
        {
            if (!ulkeId.HasValue)
                return NotFound();

            //var sehirler = _sehirService.Query().Where(s => s.UlkeId == ulkeId);
            var result = _sehirService.SehirleriGetir(ulkeId.Value);

            return Json(result.Data);
        }
    }
}
