using System.Collections.Generic;
using WebApiTest.Logic.Models.User;
using System.Threading.Tasks;

namespace WebApiTest.Logic.Definitions
{
    public interface IUserService
    {
        Task<int> InsertAsync(InsertUserDto user);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateUserDto user);
        Task<UserDto> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllAsync();
    }
}
