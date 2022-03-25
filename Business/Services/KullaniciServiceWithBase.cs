using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public interface IKullaniciService : IService<KullaniciModel, Kullanici, ETicaretContext>
    {
        Result<List<KullaniciModel>> KullanicilariGetir();
        Result<KullaniciModel> KullaniciGetir(int id);
    }

    public class KullaniciService : IKullaniciService
    {
        public RepoBase<Kullanici, ETicaretContext> Repo { get; set; }

        private readonly ETicaretContext _dbContext;
        private readonly RepoBase<KullaniciDetayi, ETicaretContext> _kullaniciDetayiRepo;
        private readonly RepoBase<Rol, ETicaretContext> _rolRepo;
        private readonly RepoBase<Ulke, ETicaretContext> _ulkeRepo;
        private readonly RepoBase<Sehir, ETicaretContext> _sehirRepo;

        public KullaniciService()
        {
            _dbContext = new ETicaretContext();
            Repo = new Repo<Kullanici, ETicaretContext>(_dbContext); // Kullanici Repository
            _kullaniciDetayiRepo = new Repo<KullaniciDetayi, ETicaretContext>(_dbContext);
            _rolRepo = new Repo<Rol, ETicaretContext>(_dbContext);
            _ulkeRepo = new Repo<Ulke, ETicaretContext>(_dbContext);
            _sehirRepo = new Repo<Sehir, ETicaretContext>(_dbContext);
        }

        public IQueryable<KullaniciModel> Query()
        {
            var kullaniciQuery = Repo.Query();
            var kullaniciDetayiQuery = _kullaniciDetayiRepo.Query();
            var rolQuery = _rolRepo.Query();
            var ulkeQuery = _ulkeRepo.Query();
            var sehirQuery = _sehirRepo.Query();

            // SQL benzeri LINQ inner join:
            var query = from kullanici in kullaniciQuery
                        join kullaniciDetayi in kullaniciDetayiQuery
                        on kullanici.Id equals kullaniciDetayi.KullaniciId
                        join rol in rolQuery
                        on kullanici.RolId equals rol.Id
                        join ulke in ulkeQuery
                        on kullaniciDetayi.UlkeId equals ulke.Id
                        join sehir in sehirQuery
                        on kullaniciDetayi.SehirId equals sehir.Id
                        //where kullanici.AktifMi // bir veya birden çok koşul and veya or ile kullanılabilir, tüm aktif veya aktif olmayan kullanıcılar gelmeli.
                        orderby rol.Adi, kullanici.KullaniciAdi // bir veya birdan çok entity özelliği kullanılabilir
                        select new KullaniciModel()
                        {
                            Id = kullanici.Id,
                            KullaniciAdi = kullanici.KullaniciAdi,
                            Sifre = kullanici.Sifre,
                            AktifMi = kullanici.AktifMi,
                            KullaniciDetayi = new KullaniciDetayiModel()
                            {
                                Cinsiyet = kullaniciDetayi.Cinsiyet,
                                Eposta = kullaniciDetayi.Eposta,
                                UlkeId = kullaniciDetayi.UlkeId,
                                UlkeAdiDisplay = ulke.Adi,
                                SehirId = kullaniciDetayi.SehirId,
                                SehirAdiDisplay = sehir.Adi,
                                Adres = kullaniciDetayi.Adres
                            },
                            RolId = kullanici.RolId,
                            RolAdiDisplay = rol.Adi,
                            AktifDisplay = kullanici.AktifMi ? "Evet" : "Hayır"
                        };
            return query;
        }

        public Result Add(KullaniciModel model)
        {
            throw new NotImplementedException();
        }

        public Result Update(KullaniciModel model)
        {
            throw new NotImplementedException();
        }

        public Result Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Repo.Dispose();
            _kullaniciDetayiRepo.Dispose();
            _rolRepo.Dispose();
            _ulkeRepo.Dispose();
            _sehirRepo.Dispose();
        }

        public Result<List<KullaniciModel>> KullanicilariGetir()
        {
            var kullanicilar = Query().ToList();
            if (kullanicilar.Count == 0)
                return new ErrorResult<List<KullaniciModel>>("Kullanıcı bulunamadı!");
            return new SuccessResult<List<KullaniciModel>>(kullanicilar.Count + " kullanıcı bulundu.", kullanicilar);
        }

        public Result<KullaniciModel> KullaniciGetir(int id)
        {
            var kullanici = Query().SingleOrDefault(k => k.Id == id);
            if (kullanici == null)
                return new ErrorResult<KullaniciModel>("Kullanıcı bulunamadı!");
            return new SuccessResult<KullaniciModel>(kullanici);
        }
    }
}
