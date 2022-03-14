using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace Business.Services
{
    public interface ISehirService : IService<SehirModel, Sehir, ETicaretContext>
    {

    }

    public class SehirService : ISehirService
    {
        public RepositoryBase<Sehir, ETicaretContext> Repository { get; set; } = new Repository<Sehir, ETicaretContext>();

        public IQueryable<SehirModel> Query()
        {
            return Repository.EntityQuery("Ulke").OrderBy(s => s.Adi).Select(s => new SehirModel()
            {
                Id = s.Id,
                Adi = s.Adi,
                UlkeId = s.UlkeId,
                UlkeDisplay = new UlkeModel()
                {
                    Adi = s.Ulke.Adi
                }
            });
        }

        public Result Add(SehirModel model)
        {
            if (Repository.EntityQuery().Any(s => s.Adi.ToUpper() == model.Adi.ToUpper().Trim()))
                return new ErrorResult("Girdiğiniz şehir adına sahip kayıt bulunmaktadır!");
            Sehir entity = new Sehir()
            {
                Adi = model.Adi.Trim(),
                UlkeId = model.UlkeId.Value
            };
            Repository.Add(entity);
            return new SuccessResult();
        }

        public Result Update(SehirModel model)
        {
            if (Repository.EntityQuery().Any(s => s.Adi.ToUpper() == model.Adi.ToUpper().Trim() && s.Id != model.Id))
                return new ErrorResult("Girdiğiniz şehir adına sahip kayıt bulunmaktadır!");
            Sehir entity = Repository.EntityQuery(s => s.Id == model.Id).SingleOrDefault();
            entity.Adi = model.Adi.Trim();
            entity.UlkeId = model.UlkeId.Value;
            Repository.Update(entity);
            return new SuccessResult();
        }

        public Result Delete(int id)
        {
            Sehir entity = Repository.EntityQuery(s => s.Id == id, "KullaniciDetaylari").SingleOrDefault();
            if (entity.KullaniciDetaylari != null && entity.KullaniciDetaylari.Count > 0)
                return new ErrorResult("Silinmek istenen şehre ait kullanıcılar bulunmaktadır!");
            Repository.DeleteEntity(id);
            return new SuccessResult();
        }

        public void Dispose()
        {
            Repository.Dispose();
        }
    }
}
