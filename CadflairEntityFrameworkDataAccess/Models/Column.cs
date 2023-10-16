using System.ComponentModel.DataAnnotations.Schema;

namespace CadflairEntityFrameworkDataAccess.Models;

public class Column
{
    public int Id { get; set; }
    public int ProductTableId { get; set; }
    public ProductTable ProductTable { get; set; } = null!;

    [Column(TypeName = "nvarchar(50)")]
    public string Header { get; set; } = string.Empty;
    public ICollection<TableValue> TableValues { get; set; } = null!;

    //public int ValueTypeId { get; set; }
    //public ValueTypeEnum ValueTypeEnum => (ValueTypeEnum)ValueTypeId;
}

//public enum ValueTypeEnum : int
//{
//    String = 1,
//    Integer = 2,
//    Decimal = 3,
//    Boolean = 4,
//}

