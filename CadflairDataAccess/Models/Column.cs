using System;

namespace CadflairDataAccess.Models
{
    public class Column
    {
        public int Id { get; set; }
        public int ProductTableId { get; set; }
        public string Header { get; set; } = string.Empty;
        public int CreatedById { get; set; }
        public DateTimeOffset CreatedOn { get; set; }


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

