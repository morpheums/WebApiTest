using WebApiTest.Data.Definitions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApiTest.Data
{
    public class EntityFrameworkRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbContext _dbContext;

        public Type ElementType => typeof(T);
        public Expression Expression => _dbContext.Set<T>().AsQueryable().Expression;
        public IQueryProvider Provider => _dbContext.Set<T>().AsQueryable().Provider;

        public EntityFrameworkRepository(
            DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Insert(T item)
        {

            _dbContext.Set<T>().Add(item);
        }

        public void Delete(T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item), "Entity to delete can't be null");
            }

            _dbContext.Set<T>().Remove(item);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().ToList();
        }
        public T GetById(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }
        public int Count()
        {
            return _dbContext.Set<T>().Count();
        }

        public IQueryable<T> GetAllQueryable()
        {
            return _dbContext.Set<T>();
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> expression)
        {
            return _dbContext.Set<T>().Where(expression);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dbContext.Set<T>().AsQueryable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dbContext.Set<T>().AsQueryable().GetEnumerator();
        }


        public IQueryable<T> Include(params Expression<Func<T, object>>[] expressions)
        {
            var query = _dbContext.Set<T>().AsQueryable();
            foreach (var exp in expressions)
            {
                query = query.Include(exp);
            }
            return query;
        }


        public T FirstOrDefaultNoTracking(Expression<Func<T, bool>> expression)
        {
            return _dbContext.Set<T>().AsNoTracking().FirstOrDefault(expression);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return _dbContext.Set<T>().FirstOrDefault(expression);
        }

        #region Async Methods
        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return _dbContext.Set<T>().FirstOrDefaultAsync(expression);
        }
        public Task<T> FirstOrDefaultNoTrackingAsync(Expression<Func<T, bool>> expression)
        {
            return _dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(expression);
        }
        public Task<int> CountAsync()
        {
            return _dbContext.Set<T>().CountAsync();
        }
        public Task<List<T>> GetAllAsync()
        {
            return _dbContext.Set<T>().ToListAsync();
        }
        public Task<T> GetByIdAsync(int id)
        {
            return _dbContext.Set<T>().FindAsync(id);
        }
        #endregion
    }
}
