using Business.Models.Filters;
using Business.Models.Reports;

namespace MvcWebUI.Models
{
    public class UrunlerRaporIndexViewModel
    {
        public List<UrunRaporModel> UrunlerRapor { get; set; }

        public UrunRaporFilterModel UrunlerFiltre { get; set; }
    }
}
