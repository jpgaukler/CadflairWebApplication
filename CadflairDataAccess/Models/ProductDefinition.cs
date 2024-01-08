using System;

namespace CadflairDataAccess.Models
{
    public class ProductDefinition
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public int? CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public string ThumbnailUri { get; set; }
        public string ForgeBucketKey { get; set; }
        public int CreatedById { get; }
        public DateTimeOffset CreatedOn { get; }
        public ProductTable ProductTable { get; set; }
    }
}