using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class ProductFamily
    {
        public int Id { get; set; } 
        public int ParentId { get; set; } 
        public int AccountId { get; set; } 
        public string DisplayName { get; set; } 
        public int CreatedById { get; set; } 
        public DateTime CreatedOn { get; set; } 
    }
}
