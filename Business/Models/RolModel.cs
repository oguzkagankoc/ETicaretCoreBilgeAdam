using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AppCore.Records.Bases;

namespace Business.Models
{
    public class RolModel : RecordBase
    {
        #region Entity'den kopyalanan özellikler
        [Required(ErrorMessage = "{0} gereklidir!")]
        [StringLength(20, ErrorMessage = "{0} en fazla {1} karakter olmalıdır!")]
        [DisplayName("Adı")]
        public string Adi { get; set; }
        #endregion

        #region Sayfanın ekstra ihtiyacı olan özellikler
        [DisplayName("Kullanıcılar")]
        public List<string> KullanicilarDisplay { get; set; }
        #endregion
    }
}
