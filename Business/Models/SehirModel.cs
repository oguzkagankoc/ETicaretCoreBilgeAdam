using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AppCore.Records.Bases;

namespace Business.Models
{
    public class SehirModel : RecordBase
    {
        #region Entity'den kopyalanan özellikler
        [Required(ErrorMessage = "{0} gereklidir!")]
        [StringLength(150, ErrorMessage = "{0} en fazla {1} karakter olmalıdır!")]
        [DisplayName("Şehir Adı")]
        public string Adi { get; set; }

        [Required(ErrorMessage = "{0} gereklidir!")]
        [DisplayName("Ülke")]
        public int? UlkeId { get; set; }
        #endregion

        #region Sayfanın ekstra ihtiyacı olan özellikler
        public UlkeModel UlkeDisplay { get; set; }
        #endregion
    }
}
