using AppCore.Records.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class UrunModel : RecordBase
    {
        #region Entity'den kopyalanan özellikler
        [Required(ErrorMessage = "{0} gereklidir!")]
        [StringLength(200, ErrorMessage = "{0} en fazla {1} karakter olmalıdır!")]
        [DisplayName("Adı")]
        public string Adi { get; set; }

        [StringLength(500, ErrorMessage = "{0} en fazla {1} karakter olmalıdır!")]
        [DisplayName("Açıklaması")]
        public string Aciklamasi { get; set; }

        [Range(0, 1000000000, ErrorMessage = "{0} {1} ile {2} aralığında olmalıdır!")] // {0}: Birim Fiyatı, {1}: 0, {2}: 1000000000
        [Required(ErrorMessage = "{0} gereklidir!")] // BirimFiyati nullable olmadığı için zorunlu, required mesajını özelleştirebilmek için data annotation'ı ekledik
        [DisplayName("Birim Fiyatı")]
        public double? BirimFiyati { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} {1} ile {2} aralığında olmalıdır!")]
        [Required(ErrorMessage = "{0} gereklidir!")]
        [DisplayName("Stok Miktarı")]
        public int? StokMiktari { get; set; }

        [DisplayName("Son Kullanma Tarihi")]
        public DateTime? SonKullanmaTarihi { get; set; }

        [DisplayName("Kategori")]
        [Required(ErrorMessage = "{0} gereklidir!")]
        public int? KategoriId { get; set; }
        #endregion

        #region Sayfanın ekstra ihtiyacı olan özellikler
        // eğer ürün model üzerinden bir kategorinin adı dışında diğer özellikleri (Id, Aciklamasi, vb.) de kullanılmak isteniyorsa bu şekilde referans tanımlanabilir ve bu referans ilgili serviste new'lenerek set edilebilir.
        //public KategoriModel KategoriDisplay { get; set; } 
        
        [DisplayName("Kategori")]
        public string KategoriAdiDisplay { get; set; }

        [DisplayName("Birim Fiyatı")]
        public string BirimFiyatiDisplay { get; set; }

        [DisplayName("Son Kullanma Tarihi")]
        public string SonKullanmaTarihiDisplay { get; set; }
        #endregion
    }
}
