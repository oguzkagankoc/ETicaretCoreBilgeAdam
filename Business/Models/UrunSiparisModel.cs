namespace Business.Models
{
    public class UrunSiparisModel
    {
        public int UrunId { get; set; }

        public UrunModel Urun { get; set; }

        public int SiparisId { get; set; }

        public SiparisModel Siparis { get; set; }
    }
}
