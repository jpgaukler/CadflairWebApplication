using System.ComponentModel.DataAnnotations.Schema;

namespace CadflairEntityFrameworkDataAccess.Models;

public class TableValue
{
    public int Id { get; set; }
    public int ColumnId { get; set; }
    public Column Column { get; set; } = null!;
    public int RowId { get; set; }
    public Row Row { get; set; } = null!;

    /// <summary>
    /// starting with only string values for the demo
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    public string Value { get; set; } = string.Empty;

    //public int ValueTypeId { get; set; }
    //public ValueTypeEnum ValueTypeEnum => (ValueTypeEnum)ValueTypeId; 
    //public string? StringValue { get; set; } 
    //public decimal? DecimalValue { get; set; } 
    //public int? IntegerValue { get; set; } 
    //public bool? BooleanValue { get; set; } 
}

