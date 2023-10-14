namespace CadflairBlazorServer.Pages.McMaster_Idea.Models;

public class ProductTable
{
    public int Id { get; set; }
    public int ProductDefinitionId { get; set; }
    public List<ColumnDefinition> ColumnDefinitions { get; set; } = new();
    public List<Product> Products { get; set; } = new();
}
