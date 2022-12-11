using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class SubscriptionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
