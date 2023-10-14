namespace CadflairBlazorServer.Pages.McMaster_Idea.Models;

public class ColumnValue
{
    public int Id { get; set; }
    public int ColumnDefinitionId { get; set; }
    public int ProductId { get; set; }

    /// <summary>
    /// starting with only string values for the demo
    /// </summary>
    public string Value { get; set; } = string.Empty;

    //public int ValueTypeId { get; set; }
    //public ValueTypeEnum ValueTypeEnum => (ValueTypeEnum)ValueTypeId; 
    //public string? StringValue { get; set; } 
    //public decimal? DecimalValue { get; set; } 
    //public int? IntegerValue { get; set; } 
    //public bool? BooleanValue { get; set; } 
}

