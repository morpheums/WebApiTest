using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiTest.Logic.Definitions;
using WebApiTest.Logic.Exceptions;
using WebApiTest.Logic.Models.Authentication;
using WebApiTest.WebApi.Utils;

namespace WebApiTest.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    [RoutePrefix("api/authentication")]
    public class AuthenticationController : ApiController
    {
        private readonly IAuthenticationService _authenticationService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationService"></param>
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this._authenticationService = authenticationService;
        }

        /// <summary>
        /// Endpoint to register a new user with basic information
        /// </summary>
        /// <param name="userToRegister">Information of the new user to be registered</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        public async Task<IHttpActionResult> Register([FromBody] RegisterUserDto userToRegister)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _authenticationService.RegisterAsync(userToRegister);
                    return Ok();
                }
                catch (DuplicateEntityException ex)
                {
                    return Content(HttpStatusCode.Conflict, ex.Message);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Endpoint to log into the system
        /// </summary>
        /// <param name="userCredentials">Credentials of the user to be authenticated</param>
        [HttpPost]
        [Route("Login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginDto userCredentials)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _authenticationService.LoginAsync(userCredentials);
                    var token = TokenGenerator.GenerateTokenJwt(userCredentials.Username);
                    return Ok(token);
                }
                catch (NotFoundException ex)
                {
                    return Content(HttpStatusCode.Unauthorized, ex.Message);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Endpoint to change a password of an user
        /// </summary>
        /// <param name="changePasswordInfo">Information of the user account and password</param>
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordDto changePasswordInfo)
        {
            if (ModelState.IsValid)
            {
                await _authenticationService.ChangePasswordAsync(changePasswordInfo);
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
