namespace CadflairBlazorServer.Pages.McMaster_Idea.Models;

public class Category
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public int? ThumbnailId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Thumbnail? Thumbnail { get; set; } 


    public Category? ParentCategory { get; set; } 
    /// <summary>
    /// There may be sub categories for further grouping.
    /// </summary>
    public List<Category> ChildCategories { get; set; } = new();
    public List<ProductDefinition> ProductDefinitions { get; set; } = new();
}
