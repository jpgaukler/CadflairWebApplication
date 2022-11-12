using System.ComponentModel.DataAnnotations;

namespace CadflairBlazorServer.FormModels
{
    public class LoginForm
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
