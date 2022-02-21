using AppCore.Business.Models.Results;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using Business.Services.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories.Bases;

namespace Business.Services
{
    // Program.cs'de IoC Container'da kullanabilmek için oluşturulmalı
    public class KategoriService : IKategoriService
    {
        public RepositoryBase<Kategori, ETicaretContext> Repository { get; set; }

        public KategoriService(KategoriRepositoryBase repository) // Repository'nin servise enjekte edilmesi. Ancak bu projede repository'yi new'leyerek kullanağız. Dolayısıyla repository'lerdeki DbContext de new'lenecek.
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

        // Sadece örneğin select Id, Adi, Aciklamasi from Kategoriler order by Adi sorgusunu oluşturur.
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
