
namespace BlazorAppTest3.Models
{
    public class AccountModel
    {
        public int AccountId { get; set; }
        public int SubscriptionId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int Owner { get; set; }
    }
}
