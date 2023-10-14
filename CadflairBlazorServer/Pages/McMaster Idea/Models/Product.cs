namespace CadflairBlazorServer.Pages.McMaster_Idea.Models;

public class Product
{
    public int Id { get; set; }
    public int ProductDefinitionId { get; set; }
    public List<ColumnValue> ColumnValues { get; set; } = new();
}
