using AppCore.Records.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class MagazaModel : RecordBase
    {
        #region Entity'den kopyalanan özellikler
        [Required(ErrorMessage = "{0} gereklidir!")]
        [StringLength(300, ErrorMessage = "{0} en fazla {1} karakter olmalıdır!")]
        [DisplayName("Adı")]
        public string Adi { get; set; }

        [DisplayName("Sanal")]
        public bool SanalMi { get; set; }
        #endregion

        #region Sayfanın ekstra ihtiyacı olan özellikler
        [DisplayName("Sanal")]
        public string SanalMiDisplay { get; set; }
        #endregion
    }
}
