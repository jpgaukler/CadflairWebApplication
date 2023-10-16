using System;

namespace CadflairDataAccess.Models
{
    public class TableValue
    {
        public int Id { get; set; }
        public int ProductTableId { get; set; }
        public int ColumnId { get; set; }
        public int RowId { get; set; }

        /// <summary>
        /// starting with only string values for the demo
        /// </summary>
        public string Value { get; set; } = string.Empty;
        public int CreatedById { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        //public int ValueTypeId { get; set; }
        //public ValueTypeEnum ValueTypeEnum => (ValueTypeEnum)ValueTypeId; 
        //public string? StringValue { get; set; } 
        //public decimal? DecimalValue { get; set; } 
        //public int? IntegerValue { get; set; } 
        //public bool? BooleanValue { get; set; } 
    }
}