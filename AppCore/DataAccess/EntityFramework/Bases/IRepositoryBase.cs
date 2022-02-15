using AppCore.Records.Bases;
using Microsoft.EntityFrameworkCore;

namespace AppCore.DataAccess.EntityFramework.Bases
{
    //public interface IRepositoryBase<TEntity>
    //public interface IRepositoryBase<TEntity> where TEntity : class // TEntity referans tip olabilir
    //public interface IRepositoryBase<TEntity> where TEntity : class, new() // TEntity new'lenebilen referans tip olabilir
    public interface IRepositoryBase<TEntity, TDbContext> : IDisposable where TEntity : RecordBase, new() where TDbContext : DbContext, new() // TEntity new'lenebilen ve RecordBase'den türeyen olabilir, TDbContext new'lenebilen ve DbContext'ten türeyen olabilir
    {
        TDbContext DbContext { get; set; }
        IQueryable<TEntity> Query(); // Read
        void Add(TEntity entity, bool save = true); // Create
        void Update(TEntity entity, bool save = true); // Update
        void Delete(TEntity entity, bool save = true, bool softDelete = false); // Delete
        int Save();
    }
}
