using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class Notification
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the event that will fire the notification.
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Determines if this notification should be enabled by default when creating a corresponding NotificationSetting for a new User.
        /// </summary>
        public bool EnabledByDefault { get; set; }

        /// <summary>
        /// Date and time that the record was created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }


    }
}
