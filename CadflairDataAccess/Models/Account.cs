using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string SubDirectory { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public int OwnerId { get; set; }
        public int AccountTypeId { get; set; }
        public DateTime SubscriptionExpiresOn { get; set; }

    }
}
