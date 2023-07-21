using System;

namespace CadflairDataAccess.Models
{
    public class CadModel
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique identifier used to identify a model from a url.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Foreign key to <b>Subscription</b> table. Represents the Subscription that the Product belongs to.
        /// </summary>
        public int SubscriptionId { get; set; }

        /// <summary>
        /// Foreign key to <b>ProductFolder</b> table. Represents the ProductFolder that the Product belongs to.
        /// </summary>
        public int ProductFolderId { get; set; }

        /// <summary>
        /// Foreign key to <b>User</b> table. Represents the id of the User that created the Product.
        /// </summary>
        public int CreatedById { get; set; }

        /// <summary>
        /// Name that is used in the UI.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Optional description of a model.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Bucket key for the files that are stored in Autodesk Forge OSS. 
        /// </summary>
        public string BucketKey { get; set; }

        /// <summary>
        /// Object key (Autodesk Forge OSS) for the file containing the CAD model. Could be a single file or a zip containing multiple files.
        /// </summary>
        public string ObjectKey { get; set; }

        /// <summary>
        /// Indicates if the model file (stored in Autodesk Forge OSS) is a zip file.
        /// </summary>
        public bool IsZip { get; set; }

        /// <summary>
        /// Optional name of the root model file if the stored file is a zip (used for assembly files). 
        /// </summary>
        public string RootFileName { get; set; }

        /// <summary>
        /// Date and time that the record was created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }


    }
}
