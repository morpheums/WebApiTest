using WebApiTest.Data.Definitions;
using WebApiTest.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiTest.Data
{
    public class EntityFrameworkUoW : IGenericUoW
    {
        private Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        private readonly DbContext _dbContext;

        public EntityFrameworkUoW(DbContext dbContext)
        {
            _dbContext = dbContext;
            LoadRepositories();
        }

        private void LoadRepositories()
        {
            Repository<User>();
            Repository<Address>();
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories.Keys.Contains(typeof(T)))
            {
                return _repositories[typeof(T)] as IGenericRepository<T>;
            }

            IGenericRepository<T> genericRepository = new EntityFrameworkRepository<T>(_dbContext);
            _repositories.Add(typeof(T), genericRepository);
            return genericRepository;
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
