using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace Business.Services
{
    public interface IUlkeService : IService<UlkeModel, Ulke, ETicaretContext>
    {

    }

    public class UlkeService : IUlkeService
    {
        public RepositoryBase<Ulke, ETicaretContext> Repository { get; set; } = new Repository<Ulke, ETicaretContext>();

        public IQueryable<UlkeModel> Query()
        {
            return Repository.EntityQuery().OrderBy(u => u.Adi).Select(u => new UlkeModel()
            {
                Id = u.Id,
                Adi = u.Adi
            });
        }

        public Result Add(UlkeModel model)
        {
            if (Repository.EntityQuery().Any(u => u.Adi.ToUpper() == model.Adi.ToUpper().Trim()))
                return new ErrorResult("Girdiğiniz ülke adına sahip kayıt bulunmaktadır!");
            Ulke entity = new Ulke()
            {
                Adi = model.Adi.Trim()
            };
            Repository.Add(entity);
            return new SuccessResult();
        }

        public Result Update(UlkeModel model)
        {
            if (Repository.EntityQuery().Any(u => u.Adi.ToUpper() == model.Adi.ToUpper().Trim() && u.Id != model.Id))
                return new ErrorResult("Girdiğiniz ülke adına sahip kayıt bulunmaktadır!");
            Ulke entity = Repository.EntityQuery(u => u.Id == model.Id).SingleOrDefault();
            entity.Adi = model.Adi.Trim();
            Repository.Update(entity);
            return new SuccessResult();
        }

        public Result Delete(int id)
        {
            Ulke entity = Repository.EntityQuery(u => u.Id == id, "Sehirler", "KullaniciDetaylari").SingleOrDefault();
            if (entity.Sehirler != null && entity.Sehirler.Count > 0)
                return new ErrorResult("Silinmek istenen ülkeye ait şehirler bulunmaktadır!");
            if (entity.KullaniciDetaylari != null && entity.KullaniciDetaylari.Count > 0)
                return new ErrorResult("Silinmek istenen ülkeye ait kullanıcılar bulunmaktadır!");
            Repository.DeleteEntity(id);
            return new SuccessResult();
        }

        public void Dispose()
        {
            Repository.Dispose();
        }
    }
}
