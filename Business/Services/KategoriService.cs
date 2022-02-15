using AppCore.Business.Models.Results;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using Business.Services.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace Business.Services
{
    public class KategoriService : IKategoriService
    {
        public RepositoryBase<Kategori, ETicaretContext> Repository { get; set; } 

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
            throw new NotImplementedException();
        }

        public IQueryable<KategoriModel> Query()
        {
            throw new NotImplementedException();
        }

        public Result Update(KategoriModel model)
        {
            throw new NotImplementedException();
        }
    }
}
