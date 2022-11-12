using System.ComponentModel.DataAnnotations;

namespace CadflairBlazorServer.FormModels
{
    public class RegisterForm
    {
        [Required]
        public string UserRoleName { get; set; } = string.Empty;

        [Required]
        [StringLength(25, ErrorMessage = "Must be between 2 and 25 characters.", MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(25, ErrorMessage = "Must be between 2 and 25 characters.", MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,20}$", ErrorMessage = "Password must contain an uppercase letter, lowercase letter, number, and special character.")]
        [StringLength(20, ErrorMessage = "Password must be between 8 and 20 characters long.", MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
