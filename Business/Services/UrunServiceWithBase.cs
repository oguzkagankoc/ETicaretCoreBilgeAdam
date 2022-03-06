using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace Business.Services
{
    public interface IUrunService : IService<UrunModel, Urun, ETicaretContext>
    {

    }

    public class UrunService : IUrunService
    {
        public RepositoryBase<Urun, ETicaretContext> Repository { get; set; } = new Repository<Urun, ETicaretContext>();

        public IQueryable<UrunModel> Query()
        {
            return Repository.EntityQuery("Kategori").OrderBy(u => u.Kategori.Adi).ThenBy(u => u.Adi).Select(u => new UrunModel()
            {
                Id = u.Id,
                Adi = u.Adi,
                Aciklamasi = u.Aciklamasi,
                BirimFiyati = u.BirimFiyati,
                SonKullanmaTarihi = u.SonKullanmaTarihi,
                StokMiktari = u.StokMiktari,
                KategoriId = u.KategoriId,
                KategoriAdiDisplay = u.Kategori.Adi
            });
        }

        public Result Add(UrunModel model)
        {
            throw new NotImplementedException();
        }

        public Result Update(UrunModel model)
        {
            throw new NotImplementedException();
        }

        public Result Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Repository.Dispose();
        }
    }
}
