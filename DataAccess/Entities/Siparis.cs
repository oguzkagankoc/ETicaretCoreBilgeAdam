using AppCore.Records.Bases;
using DataAccess.Enums;

namespace DataAccess.Entities
{
    public class Siparis : RecordBase
    {
        public DateTime Tarih { get; set; }
        public SiparisDurum Durum { get; set; }
        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }
        public List<UrunSiparis> UrunSiparisler { get; set; }
    }
}
