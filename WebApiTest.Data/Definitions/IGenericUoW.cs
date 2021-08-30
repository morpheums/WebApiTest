using System.Threading.Tasks;

namespace WebApiTest.Data.Definitions
{
    public interface IGenericUoW
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
