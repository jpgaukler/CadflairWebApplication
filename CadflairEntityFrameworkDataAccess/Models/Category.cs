using System.ComponentModel.DataAnnotations.Schema;

namespace CadflairEntityFrameworkDataAccess.Models;

public class Category
{
    public int Id { get; set; }
    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(500)")]
    public string? Description { get; set; } 


    public int? ThumbnailId { get; set; }
    public Thumbnail? Thumbnail { get; set; } 


    /// <summary>
    /// There may be sub categories for further grouping.
    /// </summary>
    public ICollection<Category> ChildCategories { get; set; } = null!;
    public ICollection<ProductDefinition> ProductDefinitions { get; set; } = null!;
}
