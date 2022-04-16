using System.ComponentModel;

namespace Business.Models.Filters
{
    public class UrunRaporFilterModel
    {
        [DisplayName("Kategori")]
        public int? KategoriId { get; set; }
    }
}
