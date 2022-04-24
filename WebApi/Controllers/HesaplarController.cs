using AppCore.Business.Utils.JsonWebToken.Bases;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HesaplarController : ControllerBase
    {
        private readonly IHesapService _hesapService;
        private readonly JwtUtilBase _jwtUtil;

        public HesaplarController(IHesapService hesapService, JwtUtilBase jwtUtil)
        {
            _hesapService = hesapService;
            _jwtUtil = jwtUtil;
        }

        /* Gönderilebilecek model örnek:
        {
            "kullaniciAdi": "cagil",
            "sifre": "cagil"
        }
        */
        /* Dönebilecek model örnek:
        {
            "token": "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiIxIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6ImNhZ2lsIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJuYmYiOjE2NTA3OTE5NzAsImV4cCI6MTY1MDg3ODM3MCwiaXNzIjoid3d3LmV0aWNhcmV0Y29yZS5jb20iLCJhdWQiOiJ3d3cuZXRpY2FyZXRjb3JlLmNvbSJ9.JhvzbDATP6RVSsm1BjlEwIaQxoQXf5-zKb7kWn1-GuY",
            "expiration": "2022-04-25T12:19:30.7677885+03:00"
        }
        */
        /*
            Authorization Swagger'da Authorize dedikten sonra Bearer token yani:
            Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiIxIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6ImNhZ2lsIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJuYmYiOjE2NTA3OTE5NzAsImV4cCI6MTY1MDg3ODM3MCwiaXNzIjoid3d3LmV0aWNhcmV0Y29yZS5jb20iLCJhdWQiOiJ3d3cuZXRpY2FyZXRjb3JlLmNvbSJ9.JhvzbDATP6RVSsm1BjlEwIaQxoQXf5-zKb7kWn1-GuY
            girilerek sağlanır.
        */
        [HttpPost("Giris")]
        public IActionResult Giris(KullaniciGirisModel model) // ~/api/Hesaplar/Giris --- Post
        {
            var girisResult = _hesapService.Giris(model);
            if (!girisResult.IsSuccessful)
                return BadRequest(girisResult.Message);
            var tokenResult = _jwtUtil.CreateJwt(girisResult.Data.KullaniciAdi, girisResult.Data.RolAdiDisplay, girisResult.Data.Id.ToString());
            return Ok(tokenResult.Data);
        }
    }
}
