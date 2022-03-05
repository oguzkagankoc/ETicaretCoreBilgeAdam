using AppCore.Records.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    // Modeller entity'ler üzerinden oluşturularak kullanıcı ile etkileşimde bulunulacak sayfalarda kullanılır.
    public class KategoriModel : RecordBase
    {
        #region Entity'den kopyalanan özellikler
        //[Required]
        //[Required(ErrorMessage = "Adı gereklidir!")]
        [Required(ErrorMessage = "{0} gereklidir!")] // {0}: Adı
        //[StringLength(100)]
        [StringLength(100, ErrorMessage = "{0} en fazla {1} karakter olmalıdır!")] // {0}: Adı, {1}: 100
        [DisplayName("Adı")]
        public string Adi { get; set; }

        [DisplayName("Açıklaması")]
        [MinLength(2, ErrorMessage = "{0} en az {1} karakter olmalıdır!")] // {0}: Açıklaması, {1}: 2
        [MaxLength(4000, ErrorMessage = "{0} en çok {1} karakter olmalıdır!")] // {0}: Açıklaması, {1}: 4000
        public string Aciklamasi { get; set; }
        #endregion

        #region Sayfanın ekstra ihtiyacı olan özellikler
        [DisplayName("Ürün Sayısı")]
        public int UrunSayisiDisplay { get; set; }
        #endregion
    }
}
