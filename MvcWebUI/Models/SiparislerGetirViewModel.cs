using Business.Models;
using Business.Models.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcWebUI.Models
{
    public class SiparislerGetirViewModel
    {
        public List<SiparisModel> Siparisler { get; set; }
        public SiparisFilterModel Filtre { get; set; }

        public MultiSelectList SiparisDurumMultiSelectList => new MultiSelectList(Filtre.SiparisDurumlar, "Value", "Text");
    }
}
