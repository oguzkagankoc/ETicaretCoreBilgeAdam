using System.ComponentModel;

namespace Business.Models
{
    public class SepetElemanModel
    {
        public int UrunId { get; set; }
        public int KullaniciId { get; set; }

        [DisplayName("Ürün Adı")]
        public string UrunAdi { get; set; }

        [DisplayName("Ürün Birim Fiyatı")]
        public double UrunBirimFiyati { get; set; }
    }
}
