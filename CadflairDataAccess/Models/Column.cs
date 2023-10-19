using System;
using System.Collections.Generic;

namespace CadflairDataAccess.Models
{
    public class Column : IComparable<Column>
    {
        public int Id { get; set; }
        public int ProductTableId { get; set; }
        public string Header { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public int CreatedById { get; }
        public DateTimeOffset CreatedOn { get; }
        public List<TableValue> TableValues { get; set; } = new List<TableValue>();

        public int CompareTo(Column other)
        {
            return SortOrder.CompareTo(other.SortOrder);
        }

        //public int ValueTypeId { get; set; }
        //public ValueTypeEnum ValueTypeEnum => (ValueTypeEnum)ValueTypeId;
    }
}

//public enum ValueTypeEnum : int
//{
//    String = 1,
//    Integer = 2,
//    Decimal = 3,
//    Boolean = 4,
//}

