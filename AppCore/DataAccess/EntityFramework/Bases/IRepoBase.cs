using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCore.DataAccess.EntityFramework.Bases
{
    //public interface IRepoBase<TEntity>
    //public interface IRepoBase<TEntity> where TEntity : class // TEntity referans tip olabilir
    //public interface IRepoBase<TEntity> where TEntity : class, new() // TEntity new'lenebilen referans tip olabilir
    public interface IRepoBase<TEntity, TDbContext> : IDisposable where TEntity : class, new() where TDbContext : DbContext, new() // TEntity new'lenebilen referans tip olabilir, TDbContext new'lenebilen ve DbContext'ten türeyen olabilir
    {
        TDbContext DbContext { get; set; }
        IQueryable<TEntity> Query(params string[] entitiesToInclude); // Read
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, params string[] entitiesToInclude);
        void Add(TEntity entity, bool save = true); // Create
        void Update(TEntity entity, bool save = true); // Update
        void Delete(TEntity entity, bool save = true); // Delete
        void Delete(Expression<Func<TEntity, bool>> predicate = null, bool save = true);
        int Save();
    }
}
