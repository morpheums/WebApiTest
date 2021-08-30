using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiTest.Logic.Definitions;
using WebApiTest.Logic.Exceptions;
using WebApiTest.Logic.Models.User;

namespace WebApiTest.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Authorize]
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var data = await _userService.GetAllAsync();
            return Ok(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] InsertUserDto user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = await _userService.InsertAsync(user);
                    return Ok(userId);
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
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody] UpdateUserDto user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.UpdateAsync(user);
                }
                catch (NotFoundException)
                {
                    return NotFound();
                }

                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
