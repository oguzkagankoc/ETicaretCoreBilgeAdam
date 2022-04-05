using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class UrunSiparis
    {
        [Key]
        [Column(Order = 0)]
        public int UrunId { get; set; }

        public Urun Urun { get; set; }

        [Key]
        [Column(Order = 1)]
        public int SiparisId { get; set; }

        public Siparis Siparis { get; set; }

        public int UrunAdedi { get; set; }
    }
}
