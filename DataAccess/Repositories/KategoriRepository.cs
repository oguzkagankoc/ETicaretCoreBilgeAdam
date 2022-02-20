using DataAccess.Contexts;
using DataAccess.Repositories.Bases;

namespace DataAccess.Repositories
{
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
