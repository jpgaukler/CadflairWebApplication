using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class User
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int UserRoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string EmailAddress { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedOn { get; set; }
    }

}
