using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class ProductFamily
    {
        public int Id { get; set; } 
        public int SubscriptionId { get; set; } 
        public int ParentId { get; set; } 
        public string DisplayName { get; set; } 
        public int CreatedById { get; set; } 
        public DateTimeOffset CreatedOn { get; set; } 
    }
}
