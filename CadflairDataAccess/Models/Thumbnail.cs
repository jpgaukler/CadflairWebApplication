using System;

namespace CadflairDataAccess.Models
{
    public class Thumbnail
    {
        public int Id { get; set; }
        public string Base64String { get; set; } = string.Empty;
        //public byte[] Bytes { get; set; }
        public int CreatedById { get; }
        public DateTimeOffset CreatedOn { get; }
    }
}