using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class User
    {
        public int Id { get; set; }
        public Guid ObjectIdentifier { get; set; }
        public int SubscriptionId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string EmailAddress { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }

}
