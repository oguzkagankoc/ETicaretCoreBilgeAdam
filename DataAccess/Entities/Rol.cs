using AppCore.Records.Bases;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Rol : RecordBase
    {
        [Required]
        [StringLength(20)]
        public string Adi { get; set; }

        public List<Kullanici> Kullanicilar { get; set; }
    }
}
