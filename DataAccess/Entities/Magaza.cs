using AppCore.Records.Bases;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class Magaza : RecordBase
    {
        [Required]
        [StringLength(300)]
        public string Adi { get; set; }

        public bool SanalMi { get; set; }

        [Column(TypeName="image")]
        public byte[] Imaj { get; set; }

        [StringLength(5)]
        public string ImajDosyaUzantisi { get; set; }

        public List<UrunMagaza> UrunMagazalar { get; set; }
    }
}
