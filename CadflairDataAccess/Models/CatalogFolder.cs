using System;
using System.Collections.Generic;

namespace CadflairDataAccess.Models
{
    public class CatalogFolder : IComparable<CatalogFolder>
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; } 

        /// <summary>
        /// Foreign key to <b>Subscription</b> table. Represents the Subscription that the CatalogFolder belongs to.
        /// </summary>
        public int SubscriptionId { get; set; } 

        /// <summary>
        /// Foreign key to <b>CatalogFolder</b> table. Represents the parent CatalogFolder in the nested structure.
        /// </summary>
        public int? ParentId { get; set; } 

        /// <summary>
        /// Name that is used in the UI.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Foreign key to <b>User</b> table. Represents the id of the User that created the CatalogFolder.
        /// </summary>
        public int CreatedById { get; set; } 

        /// <summary>
        /// Date and time that the record was created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        public List<CatalogFolder> ChildFolders { get; set; } = new List<CatalogFolder>();
        public CatalogFolder ParentFolder { get; set; }

        public int CompareTo(CatalogFolder other)
        {
            return DisplayName.CompareTo(other.DisplayName);
        }
    }
}
