using System;
using System.Collections.Generic;

namespace CadflairDataAccess.Models
{
    public class Category : IComparable<Category>
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int SubscriptionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public int? ThumbnailId { get; set; }
        public int CreatedById { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public Category ParentCategory { get; set; }
        public List<Category> ChildCategories { get; set; } = new List<Category>();

        public int CompareTo(Category other)
        {
            return Name.CompareTo(other.Name);
        }

    }
}