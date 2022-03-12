using AppCore.Records.Bases;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Magaza : RecordBase
    {
        [Required]
        [StringLength(300)]
        public string Adi { get; set; }

        public bool SanalMi { get; set; }

        public List<UrunMagaza> UrunMagazalar { get; set; }
    }
}
