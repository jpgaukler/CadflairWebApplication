
namespace CadflairBlazorServer.Emails
{
    public class ProductQuoteRequestEmailModel
    {
        public string Path => "CadflairBlazorServer.Emails.ProductQuoteRequestEmail.cshtml";
        public string CustomerName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
    }
}
