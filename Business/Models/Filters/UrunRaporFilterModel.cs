using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AppCore.Business.Validations;

namespace Business.Models.Filters
{
    public class UrunRaporFilterModel
    {
        [DisplayName("Kategori")]
        public int? KategoriId { get; set; }

        [StringLength(200, ErrorMessage = "{0} en fazla {1} karakter olmalıdır!")]
        [DisplayName("Ürün Adı")]
        public string UrunAdi { get; set; }

        // double için giriş:1.2 - C# kullanım:12; giriş:1,2 - C# kullanım:1.2   *1
        // string için giriş:1.2 veya 1,2 - C# kullanım:1.2   *2
        [DisplayName("Birim Fiyatı")]
        [StringDecimal(ErrorMessage = "{0} başlangıç değeri sayısal olmalıdır!")]
        //public double? BirimFiyatiBaslangic { get; set; } // *1
        public string BirimFiyatiBaslangic { get; set; } // *2

        [DisplayName("Birim Fiyatı")]
        [StringDecimal(ErrorMessage = "{0} bitiş değeri sayısal olmalıdır!")]
        //public double? BirimFiyatiBitis { get; set; } // *1
        public string BirimFiyatiBitis { get; set; } // *2

        [DisplayName("Stok Miktarı")]
        public int? StokMiktariBaslangic { get; set; }

        public int? StokMiktariBitis { get; set; }

        [DisplayName("Son Kullanma Tarihi")] 
        public string SonKullanmaTarihiBaslangic { get; set; } // DateTime için giriş: 16.04.2022 C# kullanım:2022 (yıl) 04 (ay) 16 (gün)

        public string SonKullanmaTarihiBitis { get; set; }

        [DisplayName("Mağaza")]
        public List<int> MagazaIdleri { get; set; }
    }
}
