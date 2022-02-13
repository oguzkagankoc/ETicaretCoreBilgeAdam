using AppCore.Records.Bases;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class KategoriModel : RecordBase
    {
        #region Entity'den kopyalanan özellikler
        [Required]
        [StringLength(100)]
        public string Adi { get; set; }

        public string Aciklamasi { get; set; }
        #endregion
    }
}
