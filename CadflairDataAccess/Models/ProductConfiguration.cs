using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class ProductConfiguration
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ArgumentJson { get; set; }
        public Guid ForgeObjectKey { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
