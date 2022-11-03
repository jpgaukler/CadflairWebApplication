using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class User
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int RoleId { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
