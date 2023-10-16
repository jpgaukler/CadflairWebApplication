using System;
using System.Collections.Generic;

namespace CadflairDataAccess.Models
{
    public class Row
    {
        public int Id { get; set; }
        public int ProductTableId { get; set; }
        public int CreatedById { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public List<TableValue> TableValues { get; set; }
    }
}