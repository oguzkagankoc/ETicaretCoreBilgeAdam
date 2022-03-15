using DataAccess.Enums;
using System.ComponentModel.DataAnnotations;
using AppCore.Records.Bases;

namespace DataAccess.Entities
{
    //todo
    //public class KullaniciDetayi
    //{
    //    [Key]
    //    public int KullaniciId { get; set; }

    //    public Kullanici Kullanici { get; set; }

    //    public Cinsiyet Cinsiyet { get; set; }

    //    [Required]
    //    [StringLength(200)]
    //    public string Eposta { get; set; }

    //    public int UlkeId { get; set; }
    //    public Ulke Ulke { get; set; }

    //    public int SehirId { get; set; }
    //    public Sehir Sehir { get; set; }

    //    [Required]
    //    public string Adres { get; set; }
    //}
    public class KullaniciDetayi : RecordBase // Repository'ler için RecordBase'den türeyen kuralını tanımladığımızdan bu şekilde oluşturmalıyız
    {
        public Kullanici Kullanici { get; set; }

        public Cinsiyet Cinsiyet { get; set; }

        [Required]
        [StringLength(200)]
        public string Eposta { get; set; }

        public int UlkeId { get; set; }
        public Ulke Ulke { get; set; }

        public int SehirId { get; set; }
        public Sehir Sehir { get; set; }

        [Required]
        public string Adres { get; set; }
    }
}
