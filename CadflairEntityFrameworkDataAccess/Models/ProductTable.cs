namespace CadflairEntityFrameworkDataAccess.Models;

public class ProductTable
{
    public int Id { get; set; }
    public int ProductDefinitionId { get; set; }
    public ProductDefinition ProductDefinition { get; set; } = null!;
    public ICollection<Column> Columns { get; set; } = null!;
    public ICollection<Row> Rows { get; set; } = null!;
}
