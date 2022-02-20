using AppCore.Business.Models.Results;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using Business.Services.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories.Bases;

namespace Business.Services
{
    public class KategoriService : IKategoriService
    {
        public RepositoryBase<Kategori, ETicaretContext> Repository { get; set; }

        public KategoriService(KategoriRepositoryBase repository)
        {
            Repository = repository;
        }

        public Result Add(KategoriModel model)
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

        // Sadece örneğin select Id, Adi, Aciklamasi from Kategoriler sorgusunu oluşturur.
        public IQueryable<KategoriModel> Query()
        {
            IQueryable<KategoriModel> query = Repository.EntityQuery().OrderBy(kategori => kategori.Adi).Select(kategori => new KategoriModel()
            {
                Id = kategori.Id,
                Adi = kategori.Adi,
                Aciklamasi = kategori.Aciklamasi
            });
            return query;
        }

        public Result Update(KategoriModel model)
        {
            throw new NotImplementedException();
        }
    }
}
