namespace CadflairEntityFrameworkDataAccess.Models;

public class Row
{
    public int Id { get; set; }
    public int ProductTableId { get; set; }
    public ProductTable ProductTable { get; set; } = null!;
    public ICollection<TableValue> TableValues { get; set; } = null!;
    public ICollection<Attachment> Attachments { get; set; } = null!;
}
