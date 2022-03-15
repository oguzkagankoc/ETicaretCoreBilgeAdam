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

    }

    public class MagazaService : IMagazaService
    {
        public RepositoryBase<Magaza, ETicaretContext> Repository { get; set; } = new Repository<Magaza, ETicaretContext>();

        public IQueryable<MagazaModel> Query()
        {
            return Repository.EntityQuery().OrderBy(m => m.Adi).Select(m => new MagazaModel()
            {
                Id = m.Id,
                Adi = m.Adi,
                SanalMi = m.SanalMi,
                SanalMiDisplay = m.SanalMi ? "Evet" : "Hayır"
            });
        }

        public Result Add(MagazaModel model)
        {
            if (Repository.EntityQuery().Any(m => m.Adi.ToUpper() == model.Adi.ToUpper().Trim()))
                return new ErrorResult("Girdiğiniz mağaza adına sahip kayıt bulunmaktadır!");
            Magaza entity = new Magaza()
            {
                Adi = model.Adi.Trim(),
                SanalMi = model.SanalMi
            };
            Repository.Add(entity);
            return new SuccessResult();
        }

        public Result Update(MagazaModel model)
        {
            if (Repository.EntityQuery().Any(m => m.Adi.ToUpper() == model.Adi.ToUpper().Trim() && m.Id != model.Id))
                return new ErrorResult("Girdiğiniz mağaza adına sahip kayıt bulunmaktadır!");
            Magaza entity = Repository.EntityQuery(m => m.Id == model.Id).SingleOrDefault();
            entity.Adi = model.Adi.Trim();
            entity.SanalMi = model.SanalMi;
            Repository.Update(entity);
            return new SuccessResult();
        }

        public Result Delete(int id)
        {
            // önce ürünle ilişkili olan ürün mağaza kayıtları mağaza id'ye göre getirilir ve silinir.
            using (RepositoryBase<UrunMagaza, ETicaretContext> urunMagazaRepository = new Repository<UrunMagaza, ETicaretContext>())
            {
                List<UrunMagaza> urunMagazaEntities = urunMagazaRepository.EntityQuery(um => um.MagazaId == id).ToList();
                if (urunMagazaEntities != null && urunMagazaEntities.Count > 0)
                {
                    foreach (UrunMagaza urunMagazaEntity in urunMagazaEntities)
                    {
                        urunMagazaRepository.Delete(urunMagazaEntity, false); // birden çok ürün mağaza kaydı silinebileceği için save parametresini false gönderip veritabanında silme sorgusunu çalıştırmıyoruz, sadece ürün mağaza kaydını silinecek olarak işaretliyoruz.
                    }
                    urunMagazaRepository.Save(); // yukarıda silinecek ürün mağaza kayıtlarını işaretledikten sonra tek seferde veritabanında silme sorgularını çalıştırıyoruz (unit of work).
                }
            }
            Repository.DeleteEntity(id); // son olarak mağazamızı siliyoruz.
            return new SuccessResult();
        }

        public void Dispose()
        {
            Repository.Dispose();
        }
    }
}
