using System.ComponentModel;

namespace Business.Models
{
    public class SepetElemanGroupByModel
    {
        public int UrunId { get; set; }
        public int KullaniciId { get; set; }

        [DisplayName("Ürün Adı")]
        public string UrunAdi { get; set; }
     
        public double ToplamUrunBirimFiyati { get; set; }

        [DisplayName("Toplam Ürün Birim Fiyatı")]
        public string ToplamUrunBirimFiyatiDisplay { get; set; }

        [DisplayName("Toplam Ürün Adedi")]
        public int ToplamUrunAdedi { get; set; }
    }
}
