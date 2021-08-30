using System.ComponentModel.DataAnnotations;

namespace WebApiTest.Logic.Models.Authentication
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "{0} is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Old password is required.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters long.")]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "New password must contain at least one uppercase letter and one number.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Password confirmation is required.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string PasswordConfirmation { get; set; }
    }
}
