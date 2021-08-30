using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApiTest.Data.Definitions
{
    public interface IGenericRepository<T> : IQueryable<T> where T : class
    {
        void Insert(T item);
        void Delete(T item);
        int Count();
        T FirstOrDefault (Expression<Func<T, bool>> expression);
        T FirstOrDefaultNoTracking(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetAll();
        T GetById(int id);

        #region Async Methods 
        Task<int> CountAsync();
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);
        Task<T> FirstOrDefaultNoTrackingAsync(Expression<Func<T, bool>> expression);
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        #endregion
    }
}
