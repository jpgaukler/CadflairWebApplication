using System;
using System.Collections.Generic;

namespace CadflairDataAccess.Models
{
    public class ProductTable
    {
        public int Id { get; set; }
        public int ProductDefinitionId { get; set; }
        public int CreatedById { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        public List<Row> Rows { get; set; } = new List<Row>();
        public List<Column> Columns { get; set; } = new List<Column>();
    }
}