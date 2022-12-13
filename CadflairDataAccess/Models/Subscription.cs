using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public int SubscriptionTypeId { get; set; }
        public string CompanyName { get; set; }
        public string PageName { get; set; }
        public int CreatedById { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int OwnerId { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }

    }
}
