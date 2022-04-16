using System.ComponentModel;

namespace Business.Models.Filters
{
    public class UrunRaporFilterModel
    {
        [DisplayName("Kategori")]
        public int? KategoriId { get; set; }

        [DisplayName("Ürün Adı")]
        public string UrunAdi { get; set; }

        // double için giriş:1.2 - C# kullanım:12; giriş:1,2 - C# kullanım:1.2   *1
        // string için giriş:1.2 veya 1,2 - C# kullanım:1.2   *2
        [DisplayName("Birim Fiyatı")]
        //public double? BirimFiyatiBaslangic { get; set; } // *1
        public string BirimFiyatiBaslangic { get; set; } // *2

        //public double? BirimFiyatiBitis { get; set; } // *1
        public string BirimFiyatiBitis { get; set; } // *2

        [DisplayName("Stok Miktarı")]
        public int? StokMiktariBaslangic { get; set; }

        public int? StokMiktariBitis { get; set; }

        [DisplayName("Son Kullanma Tarihi")] 
        public string SonKullanmaTarihiBaslangic { get; set; } // DateTime için giriş: 16.04.2022 C# kullanım:2022 (yıl) 04 (ay) 16 (gün)

        public string SonKullanmaTarihiBitis { get; set; }
    }
}
