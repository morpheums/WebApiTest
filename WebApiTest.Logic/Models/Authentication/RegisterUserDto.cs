using System.ComponentModel.DataAnnotations;

namespace WebApiTest.Logic.Models.Authentication
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(16, ErrorMessage = "{0} must be between 4 and 16 characters.", MinimumLength = 4)]
        [RegularExpression(@"[^\s]+", ErrorMessage = "{0} cannot have white spaces.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(50, ErrorMessage = "{0} must be between 7 and 50 characters", MinimumLength = 7)]
        [EmailAddress(ErrorMessage = "Provided {0} is not a valid e-mail address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [MinLength(6, ErrorMessage = "{0} must be at least 6 characters long.")]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "{0} must contain at least one uppercase letter and one number.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password confirmation is required.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string PasswordConfirmation { get; set; }
    }
}