using System.ComponentModel.DataAnnotations;

namespace WebApiTest.Logic.Models.User
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "{0} is required.")]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public UserAddressDto Address { get; set; }
    }
}
