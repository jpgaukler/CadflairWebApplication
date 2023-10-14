namespace CadflairBlazorServer.Pages.McMaster_Idea.Models;

public class ProductDefinition
{
    public int Id { get; set; }
    public int? CategoryId { get; set; }
    public int? ThumbnailId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Thumbnail? Thumbnail { get; set; } 

    /// <summary>
    /// The bucket key for all attachment files stored in Forge OSS.
    /// </summary>
    public string ForgeBucketKey { get; set; } = string.Empty;
}
