using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    // https://httpstatuses.com/

    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 401 durum kodu: yetkisiz (unauthorized)
    public class UrunlerController : ControllerBase // ~/api/Urunler
    {
        private readonly IUrunService _urunService;

        public UrunlerController(IUrunService urunService)
        {
            _urunService = urunService;
        }

        // WebApi default route tanımı: controller/id?
        [HttpGet] // mutlaka yazılmalı, özellikle Swagger kullanımı için
        [AllowAnonymous]
        public IActionResult Get() // Get adı önemli, ~/api/Urunler --- Get olarak çağrıldığında otomatik HttpGet ile işaretlenen Get aksiyonu çalışır
        {
            var model = _urunService.Query().ToList();
            if (model.Count == 0)
                return NotFound(); // 404 durum kodu: kaynak bulunamadı (not found)
            return Ok(model); // 200 durum kodu: tamam (ok), eğer exception alınırsa 500 durum kodu: iç sunucu hatası (internal server error) döner
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id) // ~/api/Urunler/7 --- Get
        {
            var model = _urunService.Query().SingleOrDefault(u => u.Id == id);
            if (model == null)
                return NotFound(); // 404
            return Ok(model); // 200
        }

        // 1. yöntem: standart
        //[HttpPost] // kayıt ekleme için kullanılır
        //public IActionResult Post(UrunModel model) // ~/api/Urunler --- Post

        // 2. yöntem: aksiyon özelleştirmesi

        // I. kullanım:
        //[HttpPost]
        //[Route("Create")]

        // II. kullanım:
        /* Gönderilebilecek model örnek:
        {
            "adi": "Test 1",
            "birimFiyati": 123,
            "stokMiktari": 12,
            "sonKullanmaTarihi": "2022-04-24T00:00:00.000Z",
            "kategoriId": 1
        }
        */
        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateNewProduct(UrunModel model) // ~/api/Urunler/Create --- Post
        {
            var result = _urunService.Add(model);
            if (result.IsSuccessful)
            {
                //return Ok(model); // 200
                //return Created("~/api/Urunler/" + model.Id, model); // 201 durum kodu: oluşturuldu (created)
                return CreatedAtAction("Get", new { id = model.Id }, model);
            }

            // Internal Server Error durum kodu dönme:
            //return StatusCode(500); // 500

            // Bad Request durum kodu dönme:
            //return StatusCode(400); // 400 durum kodu: kötü istek (bad request)
            //return BadRequest(); 
            return BadRequest(result.Message);
        }

        /* Gönderilebilecek model örnek:
        {
            "id": 8,
            "adi": "Test 9",
            "aciklamasi": "Test açıklama",
            "birimFiyati": 987,
            "stokMiktari": 98,
            "sonKullanmaTarihi": "2023-04-24T00:00:00.000Z",
            "kategoriId": 2
        }
        */
        [HttpPut] // kayıt güncelleme için kullanılır
        [Authorize(Roles = "Admin")]
        public IActionResult Put(UrunModel model) // ~/api/Urunler --- Put
        {
            var result = _urunService.Update(model);
            if (result.IsSuccessful)
            {
                //return NoContent(); // 204 durum kodu: içerik yok (no content)
                return Ok(model); // 200
            }
            return BadRequest(result.Message); // 400
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id) // ~/api/Urunler/8 --- Delete
        {
            var result = _urunService.Delete(id);
            if (result.IsSuccessful)
            {
                //return NoContent(); // 204
                return Ok(id); // 200
            }
            return BadRequest(result.Message); // 400
        }
    }
}
