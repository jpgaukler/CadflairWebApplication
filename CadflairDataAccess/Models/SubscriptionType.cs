using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public enum SubscriptionTypeEnum : int
    {
        Basic = 1,
        Pro = 2,
    }

    public class SubscriptionType
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the subscription type. Example: <b>Basic</b>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Date and time that the record was created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }
    }
}
