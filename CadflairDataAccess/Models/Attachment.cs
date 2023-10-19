using System;

namespace CadflairDataAccess.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        public int RowId { get; set; }
        public string ForgeObjectKey { get; set; } = string.Empty;
        public int CreatedById { get; }
        public DateTimeOffset CreatedOn { get; }

    }
}