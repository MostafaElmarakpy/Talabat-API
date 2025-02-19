using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private Hashtable _Repositories;
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> Complete()
            => await _dbContext.SaveChangesAsync();

        public void Dispose()
            => _dbContext.Dispose();

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (_Repositories == null)
                _Repositories = new Hashtable();

            var type = typeof(TEntity).Name;

            if (!_Repositories.ContainsKey(type))
            {
                var Repository = new GenericRepository<TEntity>(_dbContext);
                _Repositories.Add(type, Repository);
            }
            return (IGenericRepository<TEntity>)_Repositories[type];
        }
    }
}
