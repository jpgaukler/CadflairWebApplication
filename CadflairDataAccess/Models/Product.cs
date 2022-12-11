using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public int ProductFamilyId { get; set; }
        public string DisplayName { get; set; }
        public string ParameterJson { get; set; }
        public Guid ForgeBucketKey { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int CreatedById { get; set; }
        public bool IsPublic { get; set; }
        public bool IsConfigurable { get; set; }
    }
}
