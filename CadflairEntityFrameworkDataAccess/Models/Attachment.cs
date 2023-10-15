using System.ComponentModel.DataAnnotations.Schema;

namespace CadflairEntityFrameworkDataAccess.Models;

public class Attachment
{
    public int Id { get; set; }
    public int RowId { get; set; }
    public Row Row { get; set; } = null!;

    [Column(TypeName = "varchar(50)")]
    public string ForgeObjectKey { get; set; } = string.Empty;
}

