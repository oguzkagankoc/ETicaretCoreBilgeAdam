using Business.Models.Filters;
using Business.Models.Reports;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcWebUI.Models
{
    public class UrunlerRaporIndexViewModel
    {
        public List<UrunRaporModel> UrunlerRapor { get; set; }

        public UrunRaporFilterModel UrunlerFiltre { get; set; }

        //public SelectList KategorilerSelectList { get; set; }
    }
}
