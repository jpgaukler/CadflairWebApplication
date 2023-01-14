using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class NotificationSetting
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to <b>Notification</b> table. Represents the Notification (event) that the NotificationSetting corresponds to.
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// Foreign key to <b>User</b> table. Represents the User that the NotificationSetting corresponds to.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Determines if the corresponding notification should be sent to the corresponding user.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Date and time that the record was created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }


    }
}
