using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace Business.Services
{
    public interface IMagazaService : IService<MagazaModel, Magaza, ETicaretContext>
    {
        Result DeleteImage(int id);
    }

    public class MagazaService : IMagazaService
    {
        public RepoBase<Magaza, ETicaretContext> Repo { get; set; }

        private readonly RepoBase<UrunMagaza, ETicaretContext> _urunMagazaRepo;

        public MagazaService()
        {
            // ürün repository ve ürün mağaza repository'nin aynı DbContext üzerinde işlem yapması gerektiğinden ETicaretContext'i iki repository'e de enjekte ediyoruz
            ETicaretContext eTicaretContext = new ETicaretContext();
            Repo = new Repo<Magaza, ETicaretContext>(eTicaretContext);
            _urunMagazaRepo = new Repo<UrunMagaza, ETicaretContext>(eTicaretContext);
        }

        public IQueryable<MagazaModel> Query()
        {
            return Repo.Query().OrderBy(m => m.Adi).Select(m => new MagazaModel()
            {
                Id = m.Id,
                Adi = m.Adi,
                SanalMi = m.SanalMi,
                SanalMiDisplay = m.SanalMi ? "Evet" : "Hayır",

                Imaj = m.Imaj,
                ImajSrcDisplay = m.Imaj != null ? (m.ImajDosyaUzantisi == ".jpg" || m.ImajDosyaUzantisi == ".jpeg" ? "data:image/jpeg;base64," : "data:image/png;base64,") + Convert.ToBase64String(m.Imaj) : null,
                ImajDosyaUzantisi = m.ImajDosyaUzantisi
            });
        }

        public Result Add(MagazaModel model)
        {
            if (Repo.Query().Any(m => m.Adi.ToUpper() == model.Adi.ToUpper().Trim()))
                return new ErrorResult("Girdiğiniz mağaza adına sahip kayıt bulunmaktadır!");
            Magaza entity = new Magaza()
            {
                Adi = model.Adi.Trim(),
                SanalMi = model.SanalMi,

                Imaj = model.Imaj,
                ImajDosyaUzantisi = model.ImajDosyaUzantisi?.ToLower()
            };
            Repo.Add(entity);
            return new SuccessResult();
        }

        public Result Update(MagazaModel model)
        {
            if (Repo.Query().Any(m => m.Adi.ToUpper() == model.Adi.ToUpper().Trim() && m.Id != model.Id))
                return new ErrorResult("Girdiğiniz mağaza adına sahip kayıt bulunmaktadır!");
            Magaza entity = Repo.Query(m => m.Id == model.Id).SingleOrDefault();
            entity.Adi = model.Adi.Trim();
            entity.SanalMi = model.SanalMi;

            if (model.Imaj != null && model.Imaj.Length > 0) // eğer model üzerinden imaj gelirse güncellesin gelmezse dokunmasın
            {
                entity.Imaj = model.Imaj;
                entity.ImajDosyaUzantisi = model.ImajDosyaUzantisi.ToLower();
            }

            Repo.Update(entity);
            return new SuccessResult();
        }

        public Result Delete(int id)
        {
            // önce ürünle ilişkili olan ürün mağaza kayıtları mağaza id'ye göre getirilir ve silinir.
            // 1. yöntem:
            //List<UrunMagaza> urunMagazaEntities = urunMagazaRepo.Query(um => um.MagazaId == id).ToList();
            //foreach (UrunMagaza urunMagazaEntity in urunMagazaEntities)
            //{
            //    urunMagazaRepo.Delete(urunMagazaEntity, false); // birden çok ürün mağaza kaydı silinebileceği için save parametresini false gönderip veritabanında silme sorgusunu çalıştırmıyoruz, sadece ürün mağaza kaydını silinecek olarak işaretliyoruz.
            //}
            // 2. yöntem:
            _urunMagazaRepo.Delete(um => um.MagazaId == id, false);
            Repo.Delete(m => m.Id == id); // son olarak mağazamızı siliyoruz. save parametresi default olarak true gönderildiğinden DbContext'te SaveChanges ile değişiklikler kaydediliyor
            return new SuccessResult();
        }

        public void Dispose()
        {
            Repo.Dispose();
        }

        public Result DeleteImage(int id)
        {
            Magaza entity = Repo.Query(m => m.Id == id).SingleOrDefault();
            entity.Imaj = null;
            entity.ImajDosyaUzantisi = null;
            Repo.Update(entity);
            return new SuccessResult();
        }
    }
}
