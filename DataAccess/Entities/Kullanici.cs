using AppCore.Records.Bases;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Kullanici : RecordBase
    {
        [Required]
        [MinLength(3)]
        [MaxLength(15)]
        public string KullaniciAdi { get; set; }

        [Required]
        [StringLength(10)] // StringLength kullanılması daha uygun entity'ler için
        public string Sifre { get; set; }

        public bool AktifMi { get; set; }

        public KullaniciDetayi KullaniciDetayi { get; set; } // KullaniciDetayi ile Kullanici arasında 1 to 1 ilişki olduğu için KullaniciDetayi'da KullaniciId (primary key olarak) ve Kullanici, Kullanici'da da KullaniciDetayi tanımlanmalıdır. 

        public int RolId { get; set; } // Rol ile Kullanici arasında 1 to Many ilişki olduğu için Rol'da List of Kullanici, Kullanici'da da RolId ve Rol tanımlanmalıdır. 
        public Rol Rol { get; set; }

        public List<Siparis> Siparisler { get; set; }
    }
}
