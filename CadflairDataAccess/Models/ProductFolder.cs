using System;
using System.Collections.Generic;

namespace CadflairDataAccess.Models
{
    public class ProductFolder : IComparable
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; } 

        /// <summary>
        /// Foreign key to <b>Subscription</b> table. Represents the Subscription that the ProductFolder belongs to.
        /// </summary>
        public int SubscriptionId { get; set; } 

        /// <summary>
        /// Foreign key to <b>ProductFolder</b> table. Represents the parent ProductFolder in the nested structure.
        /// </summary>
        public int? ParentId { get; set; } 

        /// <summary>
        /// Name that is used in the UI.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Foreign key to <b>User</b> table. Represents the id of the User that created the ProductFolder.
        /// </summary>
        public int CreatedById { get; set; } 

        /// <summary>
        /// Date and time that the record was created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        public List<ProductFolder> ChildFolders { get; set; } = new List<ProductFolder>();
        public ProductFolder ParentFolder { get; set; }

        public int CompareTo(object obj)
        {
            return DisplayName.CompareTo(((ProductFolder)obj).DisplayName);
        }
    }
}
