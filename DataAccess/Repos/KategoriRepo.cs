using DataAccess.Contexts;
using DataAccess.Repos.Bases;

namespace DataAccess.Repos
{
    // Program.cs'de IoC Container'da kullanabilmek için oluşturulmalı. Ancak bu projede repository'leri servislere enjekte etmek yerine new'leyerek kullanacağız. Dolayısıyla repository'lerdeki DbContext de new'lenecek.
    public class KategoriRepo : KategoriRepoBase
    {
        public KategoriRepo() : base()
        {

        }

        public KategoriRepo(ETicaretContext eTicaretContext) : base(eTicaretContext)
        {

        }
    }
}
