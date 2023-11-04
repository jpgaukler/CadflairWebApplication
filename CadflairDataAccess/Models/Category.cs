using System;
using System.Collections.Generic;
using System.Linq;

namespace CadflairDataAccess.Models
{
    public class Category : IComparable<Category>
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int SubscriptionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public string ThumbnailUri { get; set; }
        public int CreatedById { get; }
        public DateTimeOffset CreatedOn { get; }

        public Category ParentCategory { get; set; }
        public List<Category> ChildCategories { get; set; } = new List<Category>();

        public int CompareTo(Category other)
        {
            return Name.CompareTo(other.Name);
        }
    }

    public static class CategoryHelpers
    {
        public static List<Category> ToFlatList(this IEnumerable<Category> categories)
        {
            return categories.SelectMany(i => i.ChildCategories.ToFlatList()).Concat(categories).ToList();
        } 
    }
}