using System.ComponentModel;
using AppCore.Records.Bases;
using DataAccess.Enums;

namespace Business.Models
{
    public class SiparisModel : RecordBase
    {
        #region Entity'den kopyalanan özellikler
        public DateTime Tarih { get; set; }

        public SiparisDurum Durum { get; set; }

        public int KullaniciId { get; set; }
        public KullaniciModel Kullanici { get; set; }

        public List<UrunSiparisModel> UrunSiparisler { get; set; }
        #endregion

        #region Sayfanın ekstra ihtiyacı olan özellikler
        [DisplayName("Sipariş No")]
        public string SiparisNo { get; set; }

        [DisplayName("Sipariş Tarihi")]
        public string TarihDisplay { get; set; }

        /// <summary>
        /// SiparisService'de sipariş join query'sinde kullanılmak için eklendi.
        /// </summary>
        public UrunSiparisModel UrunSiparisJoin { get; set; }

        public string SiparisColor { get; set; }
        #endregion
    }
}
