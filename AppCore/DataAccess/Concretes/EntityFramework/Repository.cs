﻿using AppCore.DataAccess.Bases.EntityFramework;
using AppCore.Records.Bases;
using Microsoft.EntityFrameworkCore;

namespace AppCore.DataAccess.Concretes.EntityFramework
{
    public class Repository<TEntity, TDbContext> : RepositoryBase<TEntity, TDbContext> where TEntity : RecordBase, new() where TDbContext : DbContext, new()
    {
        public Repository() : base()
        {

        }

        public Repository(TDbContext dbContext) : base(dbContext)
        {

        }
    }
}
