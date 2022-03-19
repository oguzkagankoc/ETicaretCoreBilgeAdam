using AppCore.Records.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class KullaniciModel : RecordBase
    {
        #region Entity'den kopyalanan özellikler
        [Required(ErrorMessage = "{0} gereklidir!")]
        [MinLength(3, ErrorMessage = "{0} en az {1} karakter olmalıdır!")]
        [MaxLength(15, ErrorMessage = "{0} en çok {1} karakter olmalıdır!")]
        [DisplayName("Kullanıcı Adı")]
        public string KullaniciAdi { get; set; }

        [Required(ErrorMessage = "{0} gereklidir!")]
        [StringLength(10, ErrorMessage = "{0} en fazla {1} karakter olmalıdır!")]
        [DisplayName("Şifre")]
        public string Sifre { get; set; }

        public bool AktifMi { get; set; }

        public int RolId { get; set; }
        #endregion

        #region Sayfanın ekstra ihtiyacı olan özellikler
        public KullaniciDetayiModel KullaniciDetayiDisplay { get; set; }

        // bu class'ta sadece kullanıcının rolü üzerinden rol adını göstermemiz gerektiğinden RolModel üzerinden bunu yapabileceğimiz gibi az (tek) özellik kullanacağımızdan sadece rol adı için özellik oluşturmamız da yeterlidir.
        //public RolModel RolDisplay { get; set; }
        public string RolAdiDisplay { get; set; }
        #endregion
    }
}
