using Business.Models;
using Business.Services;
using Business.Services.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [Authorize(Roles = "Admin")]

    public class KategorilerController : ControllerBase
    {
        //private readonly ETicaretContext _context;

        //public KategorilerController(ETicaretContext context)
        //{
        //    _context = context;
        //}

        private readonly IKategoriService _kategoriService;

        public KategorilerController(IKategoriService kategoriService)
        {
            _kategoriService = kategoriService;
        }

        // GET: api/Kategoriler
        [HttpGet]
        //public IActionResult Get()
        //{
        //    return Ok(_context.Kategoriler.ToList());
        //}
        public IActionResult Get()
        {
            var model = _kategoriService.Query().ToList();
            if (model.Count == 0)
                return NotFound(); // 404
            return Ok(model); // 200
        }

        // GET: api/Kategoriler/5
        [HttpGet("{id}")]
        //public IActionResult Get(int id)
        //{
        //    var kategori = _context.Kategoriler.Find(id);
        //    if (kategori == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(kategori);
        //}
        public IActionResult Get(int id)
        {
            var model = _kategoriService.Query().SingleOrDefault(k => k.Id == id);
            if (model == null)
                return NotFound(); // 404
            return Ok(model); // 200
        }

        // POST: api/Kategoriler
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        //public IActionResult Post(Kategori kategori)
        //{
        //    _context.Kategoriler.Add(kategori);
        //    _context.SaveChanges();
        //    return CreatedAtAction("GetKategori", new { id = kategori.Id }, kategori);
        //}
        /* Gönderilebilecek model örnek:
        {
            "adi": "Test 1",
            "aciklamasi": "Test açıklama"
        }
        */
        public IActionResult Post(KategoriModel model)
        {
            var result = _kategoriService.Add(model);
            if (result.IsSuccessful)
            {
                //return Ok(model); // 200
                return CreatedAtAction("Get", new { id = model.Id }, model); // 201
            }
            return BadRequest(result.Message); // 400
        }

        // PUT: api/Kategoriler
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        //public IActionResult Put(Kategori kategori)
        //{
        //    _context.Kategoriler.Update(kategori);
        //    _context.SaveChanges();
        //    return NoContent();
        //}
        /* Gönderilebilecek model örnek:
        {
            "id": 3,
            "adi": "Test 9",
            "aciklamasi": null
        }
        */
        public IActionResult Put(KategoriModel model)
        {
            var result = _kategoriService.Update(model);
            if (result.IsSuccessful)
                return Ok(model); // 200
            return BadRequest(result.Message); // 400
        }

        // DELETE: api/Kategoriler/3
        [HttpDelete("{id}")]
        //public IActionResult Delete(int id)
        //{
        //    var kategori = _context.Kategoriler.Find(id);
        //    if (kategori == null)
        //    {
        //        return NotFound();
        //    }
        //    _context.Kategoriler.Remove(kategori);
        //    _context.SaveChanges();
        //    return NoContent();
        //}
        public IActionResult Delete(int id)
        {
            var result = _kategoriService.Delete(id);
            if (result.IsSuccessful)
                return Ok(id); // 200
            return BadRequest(result.Message); // 400
        }
    }
}
