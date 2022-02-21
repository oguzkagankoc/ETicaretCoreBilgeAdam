using DataAccess.Contexts;
using DataAccess.Repositories.Bases;

namespace DataAccess.Repositories
{
    // Program.cs'de IoC Container'da kullanabilmek için oluşturulmalı. Ancak bu projede repository'leri servislere enjekte etmek yerine new'leyerek kullanacağız. Dolayısıyla repository'lerdeki DbContext de new'lenecek.
    public class KategoriRepository : KategoriRepositoryBase
    {
        public KategoriRepository() : base()
        {

        }

        public KategoriRepository(ETicaretContext eTicaretContext) : base(eTicaretContext)
        {

        }
    }
}
