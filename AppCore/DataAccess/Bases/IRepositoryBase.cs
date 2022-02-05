namespace AppCore.DataAccess.Bases
{
    public interface IRepositoryBase<TEntity>
    {
        IQueryable<TEntity> Query();
    }
}
