using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class Subscription
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to <b>SubscriptionType</b> table. 
        /// </summary>
        public int SubscriptionTypeId { get; set; }

        /// <summary>
        /// Name of the company that the subscription belongs to.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Name that will be used in the URL for the company's product pages.
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// Foreign key to <b>User</b> table. Represents the id of the User that created the Subscription.
        /// </summary>
        public int CreatedById { get; set; }

        /// <summary>
        /// Date and time that the record was created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Foreign key to <b>User</b> table. Represents the id of the User that owns the Subscription. This user will be able to manage the Subscription.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Date and time that the subscription expires.
        /// </summary>
        public DateTimeOffset ExpiresOn { get; set; }

    }
}
