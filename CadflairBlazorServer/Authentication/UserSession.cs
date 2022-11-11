namespace CadflairBlazorServer.Authentication
{
    public class UserSession
    {
        public string Name { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
