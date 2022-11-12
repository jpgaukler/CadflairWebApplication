using System.ComponentModel.DataAnnotations;

namespace CadflairBlazorServer.FormModels
{
    public class LoginForm
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
