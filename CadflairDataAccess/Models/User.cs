using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class User
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Guid that is used to identify the user in Azure Active Directory B2C.
        /// </summary>
        public Guid ObjectIdentifier { get; set; }

        /// <summary>
        /// Foreign key to <b>Subscription</b> table. Represents the Subscription that the user is a member of. 
        /// </summary>
        public int? SubscriptionId { get; set; }

        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Full name of the user: 'FirstName LastName'
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Email address that the user can log in with.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Date and time that the record was created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }
    }

}
