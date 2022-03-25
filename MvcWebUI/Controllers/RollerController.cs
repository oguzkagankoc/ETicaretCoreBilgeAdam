using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcWebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RollerController : Controller
    {
        private readonly IRolService _rolService;

        public RollerController(IRolService rolService)
        {
            _rolService = rolService;
        }

        // GET: Roller
        public IActionResult Index()
        {
            var result = _rolService.RolleriGetir();
            ViewBag.Sonuc = result.Message;
            return View(result.Data);
        }

        // GET: Roller/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            var result = _rolService.RolGetir(id.Value);
            if (!result.IsSuccessful)
            {
                ViewBag.Sonuc = result.Message;
            }
            return View(result.Data);
        }

        // GET: Roller/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roller/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RolModel rol)
        {
            ModelState.Remove(nameof(rol.Id));
            if (ModelState.IsValid)
            {
                var result = _rolService.Add(rol);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            return View(rol);
        }

        // GET: Roller/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            var result = _rolService.RolGetir(id.Value);
            if (!result.IsSuccessful)
            {
                ViewBag.Sonuc = result.Message;
            }
            return View(result.Data);
        }

        // POST: Roller/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RolModel rol)
        {
            if (ModelState.IsValid)
            {
                var result = _rolService.Update(rol);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            return View(rol);
        }

        // GET: Roller/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return View("Hata", "Id gereklidir!");
            }
            var result = _rolService.Delete(id.Value);
            TempData["Sonuc"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
	}
}
