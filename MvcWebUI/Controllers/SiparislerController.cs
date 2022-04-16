using System.Security.Claims;
using Business.Models;
using Business.Models.Filters;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcWebUI.Models;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace MvcWebUI.Controllers
{
    [Authorize]
    public class SiparislerController : Controller
    {
        private readonly ISiparisService _siparisService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public SiparislerController(ISiparisService siparisService, IHttpContextAccessor httpContextAccessor)
        {
            _siparisService = siparisService;

            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Al()
        {
            string sepetJson = HttpContext.Session.GetString("sepet");
            if (string.IsNullOrWhiteSpace(sepetJson))
                return RedirectToAction("Index", "Urunler");
            List<SepetElemanModel> sepet = JsonConvert.DeserializeObject<List<SepetElemanModel>>(sepetJson);

            // session'dan alınan sepetin sisteme giriş yapan kullanıcı Id'sine göre filtrelenmesi:
            List<SepetElemanModel> kullaniciSepeti = sepet.Where(s => s.KullaniciId == Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value)).ToList();

            SiparisModel siparis = new SiparisModel()
            {
                KullaniciId = kullaniciSepeti.FirstOrDefault().KullaniciId, // kullanıcı sepetindeki tüm elemanlar aynı kullanıcı Id'ye sahip olduğundan ilk elemanın kullanıcı Id'sini kullanıyoruz
                UrunSiparisler = kullaniciSepeti.Select(s => new UrunSiparisModel()
                {
                    UrunId = s.UrunId
                }).ToList()
            };
            
            // session'dan kullanıcıya ait sepetin temizlenmesi:
            foreach (SepetElemanModel eleman in kullaniciSepeti)
            {
                sepet.Remove(eleman);
            }
            sepetJson = JsonConvert.SerializeObject(sepet);
            HttpContext.Session.SetString("sepet", sepetJson);

            var result = _siparisService.Add(siparis);
            TempData["Sonuc"] = result.Message;
            return RedirectToAction(nameof(Getir));
        }

        public IActionResult Getir(SiparisFilterModel filtre = null)
        {
            if (User.IsInRole("Kullanıcı")) // eğer Kullanıcı rolündeyse kullanıcının sadece kendi siparişlerini görebilmesi için yeni bir filtre oluştur ve KullaniciId'yi User'dan ata
            {
                if (filtre == null) // filtre null is new'le
                    filtre = new SiparisFilterModel();
                filtre.KullaniciId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value); 
            }

            var result = _siparisService.SiparisleriGetir(filtre);
            List<SiparisModel> siparisler = result.Data;
            ViewBag.Sonuc = result.Message;
            SiparislerGetirViewModel viewModel = new SiparislerGetirViewModel()
            {
                Siparisler = siparisler,
                Filtre = filtre ?? new SiparisFilterModel()
            };
            return View(viewModel);
        }

        public IActionResult IptalEt(int? id)
        {
            if (id == null)
                return View("Hata", "Id gereklidir!");
            var result = _siparisService.Delete(id.Value);
            TempData["Sonuc"] = result.Message;
            return RedirectToAction(nameof(Getir));
        }

        [Authorize(Roles = "Admin")] // alınan siparişleri sadece Admin rolündekiler tamamlayabilir
        public IActionResult Tamamla(int? id)
        {
            if (id == null)
                return View("Hata", "Id gereklidir!");
            var result = _siparisService.Update(new SiparisModel() {Id = id.Value});
            TempData["Sonuc"] = result.Message;
            return RedirectToAction(nameof(Getir));
        }

        [Authorize(Roles = "Admin")]
        public async Task ExcelIndir()
        {
            var result = _siparisService.SiparisleriGetir();
            if (result.Data != null && result.Data.Count > 0)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage = new ExcelPackage();
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Siparişler");

                // 1. satır: sütun başlıkları
                excelWorksheet.Cells["A1"].Value = "Sipariş No";
                excelWorksheet.Cells["B1"].Value = "Kullanıcı Adı";
                excelWorksheet.Cells["C1"].Value = "Sipariş Tarihi";
                excelWorksheet.Cells["D1"].Value = "Durum";
                excelWorksheet.Cells["E1"].Value = "Kategori";
                excelWorksheet.Cells["F1"].Value = "Adı";
                excelWorksheet.Cells["G1"].Value = "Birim Fiyatı";
                excelWorksheet.Cells["H1"].Value = "Adet";
                excelWorksheet.Cells["I1"].Value = "Toplam Ürün Birim Fiyatı";
                excelWorksheet.Cells["J1"].Value = "Son Kullanma Tarihi";

                // 2. satırdan itibaren veriler
                for (int row = 0; row < result.Data.Count; row++)
                {
                    excelWorksheet.Cells["A" + (row + 2)].Value = result.Data[0].SiparisNo;
                    excelWorksheet.Cells["B" + (row + 2)].Value = result.Data[0].Kullanici.KullaniciAdi;
                    excelWorksheet.Cells["C" + (row + 2)].Value = result.Data[0].TarihDisplay;
                    excelWorksheet.Cells["D" + (row + 2)].Value = result.Data[0].Durum;
                    excelWorksheet.Cells["E" + (row + 2)].Value = result.Data[0].UrunSiparisJoin.Urun.KategoriAdiDisplay;
                    excelWorksheet.Cells["F" + (row + 2)].Value = result.Data[0].UrunSiparisJoin.Urun.Adi;
                    excelWorksheet.Cells["G" + (row + 2)].Value = result.Data[0].UrunSiparisJoin.Urun.BirimFiyatiDisplay;
                    excelWorksheet.Cells["H" + (row + 2)].Value = result.Data[0].UrunSiparisJoin.UrunAdedi;
                    excelWorksheet.Cells["I" + (row + 2)].Value = result.Data[0].ToplamUrunBirimFiyatiDisplay;
                    excelWorksheet.Cells["J" + (row + 2)].Value = result.Data[0].UrunSiparisJoin.Urun.SonKullanmaTarihiDisplay;
                }

                excelWorksheet.Cells["A:AZ"].AutoFitColumns();

                var excelData = excelPackage.GetAsByteArray();
                _httpContextAccessor.HttpContext.Response.Headers.Clear();
                _httpContextAccessor.HttpContext.Response.Clear();
                _httpContextAccessor.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                _httpContextAccessor.HttpContext.Response.Headers.Add("content-length", excelData.Length.ToString());
                _httpContextAccessor.HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=\"SiparişRaporu.xlsx\"");
                await _httpContextAccessor.HttpContext.Response.Body.WriteAsync(excelData, 0, excelData.Length);
                _httpContextAccessor.HttpContext.Response.Body.Flush();
            }
        }
    }
}
