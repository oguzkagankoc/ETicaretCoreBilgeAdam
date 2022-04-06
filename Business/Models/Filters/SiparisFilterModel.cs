using System.ComponentModel;
using DataAccess.Enums;

namespace Business.Models.Filters
{
    public class SiparisFilterModel
    {
        [DisplayName("Tarih")]
        public string TarihBaslangic { get; set; }

        public string TarihBitis { get; set; }

        [DisplayName("Kullanıcı")]
        public string KullaniciAdi { get; set; }

        public int? KullaniciId { get; set; }

        List<SiparisDurumModel> _siparisDurumlar;
        public List<SiparisDurumModel> SiparisDurumlar
        {
            get
            {
                _siparisDurumlar = new List<SiparisDurumModel>();
                Array values = Enum.GetValues(typeof(SiparisDurum));
                for (int i = 0; i < values.Length; i++)
                {
                    _siparisDurumlar.Add(new SiparisDurumModel()
                    {
                        Value = ((int)values.GetValue(i)).ToString(),
                        Text = values.GetValue(i).ToString()
                    });
                }
                return _siparisDurumlar;
            }
        }

        [DisplayName("Durum")]
        public List<int> SiparisDurumValues { get; set; }

        [DisplayName("Sipariş No")]
        public string SiparisNo { get; set; }
    }
}
