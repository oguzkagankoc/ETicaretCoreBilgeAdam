using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using Business.Models.Filters;
using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Enums;

namespace Business.Services
{
    public interface ISiparisService : IService<SiparisModel, Siparis, ETicaretContext>
    {
        Result<List<SiparisModel>> SiparisleriGetir(SiparisFilterModel filter = null);
    }

    public class SiparisService : ISiparisService
    {
        public RepoBase<Siparis, ETicaretContext> Repo { get; set; } // Siparis Repository

        private readonly RepoBase<KullaniciDetayi, ETicaretContext> _kullaniciDetayiRepo;
        private readonly RepoBase<Urun, ETicaretContext> _urunRepo;
        private readonly RepoBase<Kullanici, ETicaretContext> _kullaniciRepo;
        private readonly RepoBase<Ulke, ETicaretContext> _ulkeRepo;
        private readonly RepoBase<Sehir, ETicaretContext> _sehirRepo;
        private readonly RepoBase<Kategori, ETicaretContext> _kategoriRepo;
        private readonly RepoBase<UrunSiparis, ETicaretContext> _urunSiparisRepo;

        private readonly ETicaretContext _eTicaretContext;

        public SiparisService()
        {
            _eTicaretContext = new ETicaretContext();

            Repo = new Repo<Siparis, ETicaretContext>(_eTicaretContext);
            _kullaniciDetayiRepo = new Repo<KullaniciDetayi, ETicaretContext>(_eTicaretContext);
            _urunRepo = new Repo<Urun, ETicaretContext>(_eTicaretContext);
            _kullaniciRepo = new Repo<Kullanici, ETicaretContext>(_eTicaretContext);
            _ulkeRepo = new Repo<Ulke, ETicaretContext>(_eTicaretContext);
            _sehirRepo = new Repo<Sehir, ETicaretContext>(_eTicaretContext);
            _kategoriRepo = new Repo<Kategori, ETicaretContext>(_eTicaretContext);
            _urunSiparisRepo = new Repo<UrunSiparis, ETicaretContext>(_eTicaretContext);
        }


        public IQueryable<SiparisModel> Query()
        {
            var siparisQuery = Repo.Query();
            var kullaniciQuery = _kullaniciRepo.Query();
            var kullaniciDetayiQuery = _kullaniciDetayiRepo.Query();
            var ulkeQuery = _ulkeRepo.Query();
            var sehirQuery = _sehirRepo.Query();
            var urunQuery = _urunRepo.Query();
            var kategoriQuery = _kategoriRepo.Query();
            var urunSiparisQuery = _urunSiparisRepo.Query();
            var query = from siparis in siparisQuery
                        join kullanici in kullaniciQuery
                            on siparis.KullaniciId equals kullanici.Id
                        join kullaniciDetayi in kullaniciDetayiQuery
                            on kullanici.Id equals kullaniciDetayi.KullaniciId
                        join ulke in ulkeQuery
                            on kullaniciDetayi.UlkeId equals ulke.Id
                        join sehir in sehirQuery
                            on kullaniciDetayi.SehirId equals sehir.Id
                        join urunSiparis in urunSiparisQuery
                            on siparis.Id equals urunSiparis.SiparisId
                        join urun in urunQuery
                            on urunSiparis.UrunId equals urun.Id
                        join kategori in kategoriQuery
                            on urun.KategoriId equals kategori.Id
                        select new SiparisModel()
                        {
                            Id = siparis.Id,
                            SiparisNo = "S" + siparis.Id,
                            Tarih = siparis.Tarih,
                            TarihDisplay = siparis.Tarih.ToShortDateString(),
                            Durum = siparis.Durum,
                            SiparisColor = siparis.Durum == SiparisDurum.Alındı ? "bg-warning" : (siparis.Durum == SiparisDurum.Tamamlandı ? "bg-success text-white" : "bg-danger text-white"),
                            Kullanici = new KullaniciModel()
                            {
                                KullaniciAdi = kullanici.KullaniciAdi,
                                KullaniciDetayi = new KullaniciDetayiModel()
                                {
                                    Eposta = kullaniciDetayi.Eposta,
                                    Adres = kullaniciDetayi.Adres,
                                    UlkeAdiDisplay = ulke.Adi,
                                    SehirAdiDisplay = sehir.Adi
                                }
                            },
                            // join'lerde UrunSiparisler gibi kolleksiyon referansları kullanılamaz,
                            // bu yüzden UrunSiparisJoin adında tek bir referans oluşturulmuş ve kullanılmıştır.
                            UrunSiparisJoin = new UrunSiparisModel()
                            {
                                Urun = new UrunModel()
                                {
                                    Adi = urun.Adi,
                                    StokMiktari = urun.StokMiktari,
                                    BirimFiyati = urun.BirimFiyati,
                                    BirimFiyatiDisplay = urun.BirimFiyati.ToString("C2"),
                                    SonKullanmaTarihi = urun.SonKullanmaTarihi,
                                    SonKullanmaTarihiDisplay = urun.SonKullanmaTarihi.HasValue ? urun.SonKullanmaTarihi.Value.ToShortDateString() : "",
                                    KategoriAdiDisplay = kategori.Adi
                                }
                            }
                        };
            query = query.OrderBy(q => q.Kullanici.KullaniciAdi).ThenByDescending(q => q.Tarih).ThenBy(q => q.Durum)
                .ThenBy(q => q.UrunSiparisJoin.Urun.KategoriAdiDisplay).ThenBy(q => q.UrunSiparisJoin.Urun.Adi);
            return query;
        }

        /// <summary>
        /// Sipariş alma
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result Add(SiparisModel model)
        {
            // her bir kullanıcının alındı durumunda tek bir siparişi olmalı
            if (Repo.Query().Any(s => s.KullaniciId == model.KullaniciId && s.Durum == SiparisDurum.Alındı))
                return new ErrorResult("Kullanıcı daha önce sipariş vermiştir!");
            Siparis entity = new Siparis()
            {
                Tarih = DateTime.Now,
                Durum = SiparisDurum.Alındı,
                KullaniciId = model.KullaniciId,
                UrunSiparisler = model.UrunSiparisler.Select(us => new UrunSiparis()
                {
                    UrunId = us.UrunId
                }).ToList()
            };
            Repo.Add(entity);
            return new SuccessResult("Sipariş başarıyla alındı.");
        }

        /// <summary>
        /// Sipariş tamamlama
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result Update(SiparisModel model)
        {
            Siparis entity = Repo.Query(s => s.Id == model.Id).SingleOrDefault();
            if (entity == null)
                return new ErrorResult("Sipariş bulunamadı!");
            entity.Durum = SiparisDurum.Tamamlandı;
            Repo.Update(entity);
            return new SuccessResult("Sipariş başarıyla tamamlandı.");
        }

        /// <summary>
        /// Sipariş iptal etme
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result Delete(int id)
        {
            Siparis entity = Repo.Query(s => s.Id == id).SingleOrDefault();
            if (entity == null)
                return new ErrorResult("Sipariş bulunamadı!");
            entity.Durum = SiparisDurum.İptal;
            Repo.Update(entity);
            return new SuccessResult("Sipariş başarıyla iptal edildi.");
        }

        public void Dispose()
        {
            Repo.Dispose();
        }

        Result<List<SiparisModel>> ISiparisService.SiparisleriGetir(SiparisFilterModel filtre = null)
        {
            IQueryable<SiparisModel> query = Query();
            List<SiparisModel> list;
            if (filtre != null)
            {
                if (!string.IsNullOrWhiteSpace(filtre.SiparisNo))
                    query = query.Where(q => q.SiparisNo.ToUpper().Contains(filtre.SiparisNo.ToUpper().Trim()));
                if (!string.IsNullOrWhiteSpace(filtre.KullaniciAdi))
                    query = query.Where(q => q.Kullanici.KullaniciAdi.ToUpper().Contains(filtre.KullaniciAdi.ToUpper().Trim()));
                if (!string.IsNullOrWhiteSpace(filtre.TarihBaslangic))
                {
                    DateTime tarihBaslangic = DateTime.Parse(filtre.TarihBaslangic + " 00:00:00");
                    query = query.Where(q => q.Tarih >= tarihBaslangic);
                }
                if (!string.IsNullOrWhiteSpace(filtre.TarihBitis))
                {
                    DateTime tarihBitis = DateTime.Parse(filtre.TarihBitis + " 23:59:59");
                    query = query.Where(q => q.Tarih <= tarihBitis);
                }
                if (filtre.SiparisDurumValues != null && filtre.SiparisDurumValues.Count > 0)
                    query = query.Where(q => filtre.SiparisDurumValues.Contains((int) q.Durum));
            }
            list = query.ToList();
            if (list.Count == 0)
                return new ErrorResult<List<SiparisModel>>("Sipariş bulunamadı!");
            return new SuccessResult<List<SiparisModel>>(list.DistinctBy(l => l.SiparisNo).Count() + " adet sipariş bulundu.", list);
        }
    }
}
