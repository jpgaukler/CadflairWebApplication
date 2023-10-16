using System.ComponentModel.DataAnnotations.Schema;

namespace CadflairEntityFrameworkDataAccess.Models;

public class ProductDefinition
{
    public int Id { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public int? ThumbnailId { get; set; }
    public Thumbnail? Thumbnail { get; set; } 

    [Column(TypeName = "nvarchar(50)")]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(500)")]
    public string? Description { get; set; } 

    [Column(TypeName = "varchar(50)")]
    public string? ForgeBucketKey { get; set; } 
}
