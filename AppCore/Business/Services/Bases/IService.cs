using AppCore.Business.Models.Results;
using AppCore.DataAccess.Bases.EntityFramework;
using AppCore.Records.Bases;
using Microsoft.EntityFrameworkCore;

namespace AppCore.Business.Services.Bases
{
    public interface IService<TModel, TEntity, TDbContext> : IDisposable where TModel : RecordBase, new() where TEntity : RecordBase, new() where TDbContext : DbContext, new()
    {
        RepositoryBase<TEntity, TDbContext> Repository { get; set; }
        IQueryable<TModel> Query();
        Result Add(TModel model);
        Result Update(TModel model);
        Result Delete(int id);
    }
}
