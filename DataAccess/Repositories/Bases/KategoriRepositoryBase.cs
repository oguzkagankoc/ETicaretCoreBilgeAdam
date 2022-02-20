using AppCore.DataAccess.EntityFramework.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace DataAccess.Repositories.Bases
{
    // Program.cs'de IoC Container'da kullanabilmek için oluşturulmalı.
    public abstract class KategoriRepositoryBase : RepositoryBase<Kategori, ETicaretContext>
    {
        protected KategoriRepositoryBase() : base()
        {

        }

        protected KategoriRepositoryBase(ETicaretContext eTicaretContext) : base(eTicaretContext)
        {

        }
    }
}
