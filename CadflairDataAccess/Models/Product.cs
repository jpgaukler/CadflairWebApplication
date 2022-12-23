using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class Product
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to <b>Subscription</b> table. Represents the Subscription that the Product belongs to.
        /// </summary>
        public int SubscriptionId { get; set; }

        /// <summary>
        /// Foreign key to <b>ProductFolder</b> table. Represents the ProductFolder that the Product belongs to.
        /// </summary>
        public int ProductFolderId { get; set; }

        /// <summary>
        /// Name that is used in the UI.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Name that will be used in the URL for product's page.
        /// </summary>
        public string SubdirectoryName { get; set; }

        /// <summary>
        /// Json object that contains the form definition for the Product. This is derived from the iLogic form in the Inventor model.
        /// </summary>
        public string ILogicFormJson { get; set; }

        /// <summary>
        /// Bucket key for this Product in Autodesk Forge OSS. This bucket is used to stored all configuration objects and their related files.
        /// </summary>
        public Guid ForgeBucketKey { get; set; }

        /// <summary>
        /// Date and time that the record was created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Foreign key to <b>User</b> table. Represents the id of the User that created the Product.
        /// </summary>
        public int CreatedById { get; set; }

        /// <summary>
        /// Boolean indicating if this Product should appear on a public web page.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Boolean that indicates if this Product will allow custom configurations through the form definition that is provided in the ParameterJson field.
        /// </summary>
        public bool IsConfigurable { get; set; }
    }
}
