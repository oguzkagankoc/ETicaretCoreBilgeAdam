using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace Business.Services
{
    public interface IRolService : IService<RolModel, Rol, ETicaretContext>
    {
        Result<List<RolModel>> RolleriGetir();
        Result<RolModel> RolGetir(int id);
    }

    public class RolService : IRolService
    {
        public RepoBase<Rol, ETicaretContext> Repo { get; set; } = new Repo<Rol, ETicaretContext>();

        public IQueryable<RolModel> Query()
        {
            return Repo.Query("Kullanicilar").OrderBy(r => r.Adi).Select(r => new RolModel()
            {
                Id = r.Id,
                Adi = r.Adi,
                KullanicilarDisplay = r.Kullanicilar.Select(k => k.KullaniciAdi).ToList()
            });
        }

        public Result Add(RolModel model)
        {
            if (Repo.Query().Any(r => r.Adi.ToUpper() == model.Adi.ToUpper().Trim()))
                return new ErrorResult("Girdiğiniz rol adına sahip kayıt bulunmaktadır!");
            Rol entity = new Rol()
            {
                Adi = model.Adi.Trim()
            };
            Repo.Add(entity);
            return new SuccessResult();
        }

        public Result Update(RolModel model)
        {
            if (Repo.Query().Any(r => r.Adi.ToUpper() == model.Adi.ToUpper().Trim() && r.Id != model.Id))
                return new ErrorResult("Girdiğiniz rol adına sahip kayıt bulunmaktadır!");
            Rol entity = Repo.Query(r => r.Id == model.Id).SingleOrDefault();
            entity.Adi = model.Adi.Trim();
            Repo.Update(entity);
            return new SuccessResult();
        }

        public Result Delete(int id)
        {
            Rol entity = Repo.Query(r => r.Id == id, "Kullanicilar").SingleOrDefault();
            if (entity.Kullanicilar != null && entity.Kullanicilar.Count > 0)
                return new ErrorResult("Silinmek istenen role ait kullanıcılar bulunmaktadır!");
            Repo.Delete(entity);
            return new SuccessResult();
        }

        public void Dispose()
        {
            Repo.Dispose();
        }

        public Result<List<RolModel>> RolleriGetir()
        {
            var roller = Query().ToList();
            if (roller.Count == 0)
                return new ErrorResult<List<RolModel>>("Rol bulunamadı!");
            return new SuccessResult<List<RolModel>>(roller.Count + " rol bulundu.", roller);
        }

        public Result<RolModel> RolGetir(int id)
        {
            var rol = Query().SingleOrDefault(r => r.Id == id);
            if (rol == null)
                return new ErrorResult<RolModel>("Rol bulunamadı!");
            return new SuccessResult<RolModel>(rol);
        }
    }
}
