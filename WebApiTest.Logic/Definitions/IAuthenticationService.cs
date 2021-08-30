using System.Threading.Tasks;
using WebApiTest.Logic.Models.Authentication;

namespace WebApiTest.Logic.Definitions
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(LoginDto userCredentials);
        Task ChangePasswordAsync(ChangePasswordDto changePasswordInfo);
        Task RegisterAsync(RegisterUserDto userToRegister);
    }
}
