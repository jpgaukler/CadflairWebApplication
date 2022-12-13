using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public enum SubScriptionTypeEnum : int
    {
        Basic = 1,
        Pro = 2,
    }

    public class SubscriptionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
