using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AppCore.Records.Bases;

namespace Business.Models
{
    public class UlkeModel : RecordBase
    {
        #region Entity'den kopyalanan özellikler
        [Required(ErrorMessage = "{0} gereklidir!")]
        [StringLength(100, ErrorMessage = "{0} en fazla {1} karakter olmalıdır!")]
        [DisplayName("Ülke Adı")]
        public string Adi { get; set; }
        #endregion
    }
}
