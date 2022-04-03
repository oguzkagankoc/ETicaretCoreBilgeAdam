using AppCore.Records.Bases;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    //[Table("ETicaretUrunler")] // ETicaretContext OnModelCreating methodunda yapmak daha uygun
    public class Urun : RecordBase
    {
        [Required]
        [StringLength(200)]
        public string Adi { get; set; }

        [StringLength(500)]
        public string Aciklamasi { get; set; }

        public double BirimFiyati { get; set; }

        public int StokMiktari { get; set; }

        public DateTime? SonKullanmaTarihi { get; set; }

        public int KategoriId { get; set; }
        public Kategori Kategori { get; set; }

        [StringLength(255)]
        public string ImajDosyaYolu { get; set; }

        public List<UrunMagaza> UrunMagazalar { get; set; }

        // UrunSipari ile Urun arasında Many to Many ilişki olduğu için Urun ve Siparis'de List of UrunSiparis, UrunSiparis'de de UrunId (primary key olarak) ve Urun ile SiparisId (primary key olarak) ve Siparis tanımlanmalıdır.
        public List<UrunSiparis> UrunSiparisler { get; set; }
    }
}
