using System.ComponentModel;

namespace Business.Models.Reports
{
    public class UrunRaporModel
    {
        public int? KategoriId { get; set; }

        [DisplayName("Kategori")]
        public string KategoriAdi { get; set; }

        public string KategoriAciklamasi { get; set; }

        [DisplayName("Ürün")]
        public string UrunAdi { get; set; }

        public string UrunAciklamasi { get; set; }

        [DisplayName("Birim Fiyatı")]
        public string UrunBirimFiyatiDisplay { get; set; }

        [DisplayName("Stok Miktarı")]
        public int UrunStokMiktari { get; set; }

        [DisplayName("Son Kullanma Tarihi")]
        public string UrunSonKullanmaTarihiDisplay { get; set; }

        public int? MagazaId { get; set; }

        [DisplayName("Mağaza")]
        public string MagazaAdi { get; set; }
    }
}
