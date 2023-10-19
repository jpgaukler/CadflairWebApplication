using System;
using System.Collections.Generic;

namespace CadflairDataAccess.Models
{
    public class ProductTable
    {
        public int Id { get; set; }
        public int ProductDefinitionId { get; set; }
        public int CreatedById { get; }
        public DateTimeOffset CreatedOn { get; }

        public List<Row> Rows { get; set; } = new List<Row>();
        public List<Column> Columns { get; set; } = new List<Column>();
    }
}