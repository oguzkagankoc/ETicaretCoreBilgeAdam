using AppCore.Records.Bases;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    //todo
    //public class UrunMagaza
    //{
    //    [Key]
    //    [Column(Order = 0)]
    //    public int UrunId { get; set; }

    //    public Urun Urun { get; set; }

    //    [Key]
    //    [Column(Order = 1)]
    //    public int MagazaId { get; set; }

    //    public Magaza Magaza { get; set; }
    //}
    public class UrunMagaza : RecordBase // Repository'ler için RecordBase'den türeyen kuralını tanımladığımızdan bu şekilde oluşturmalıyız
    {
        public int UrunId { get; set; }

        public Urun Urun { get; set; }

        public int MagazaId { get; set; }

        public Magaza Magaza { get; set; }
    }
}
