using System.ComponentModel;
using AppCore.Business.Models.Ordering;
using AppCore.Business.Models.Paging;
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

        public PageModel Sayfa { get; set; }

        public SelectList SayfalarSelectList { get; set; }

        [DisplayName("Sıra")]
        public OrderModel Sira { get; set; }

        public SelectList SiraSutunBasliklariSelectList { get; set; }

        public SelectList SiraYonSelectList { get; set; }

        public MultiSelectList MagazalarMultiSelectList { get; set; }
    }
}
