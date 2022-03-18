using AppCore.DataAccess.EntityFramework.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace DataAccess.Repos.Bases
{
    // Program.cs'de IoC Container'da kullanabilmek için oluşturulmalı. Ancak bu projede repository'leri servislere enjekte etmek yerine new'leyerek kullanacağız. Dolayısıyla repository'lerdeki DbContext de new'lenecek.
    public abstract class KategoriRepoBase : RepoBase<Kategori, ETicaretContext>
    {
        protected KategoriRepoBase() : base()
        {

        }

        protected KategoriRepoBase(ETicaretContext eTicaretContext) : base(eTicaretContext)
        {

        }
    }
}
