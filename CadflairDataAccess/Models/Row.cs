using System;
using System.Collections.Generic;

namespace CadflairDataAccess.Models
{
    public class Row
    {
        public int Id { get; set; }
        public int ProductTableId { get; set; }
        public string PartNumber { get; set; }
        public int CreatedById { get; }
        public DateTimeOffset CreatedOn { get; }
        public List<TableValue> TableValues { get; set; } = new List<TableValue>();
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}