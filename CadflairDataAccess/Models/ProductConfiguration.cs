using System;

namespace CadflairDataAccess.Models
{
    public class ProductConfiguration
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to <b>ProductVersion</b> table. Represents the ProductVersion that the ProductConfiguration belongs to.
        /// </summary>
        public int ProductVersionId { get; set; }

        /// <summary>
        /// Boolean indicating if this ProductConfiguration is the default configuration that will appear on the webpage for the product.
        /// </summary>
        public bool IsDefault{ get; set; }

        /// <summary>
        /// Json object that contains the input values that were provided for the ProductConfiguration.
        /// </summary>
        public string ArgumentJson { get; set; }

        /// <summary>
        /// Bucket key for the files that are stored in Autodesk Forge OSS. 
        /// </summary>
        public string BucketKey { get; set; }

        /// <summary>
        /// Object key (Autodesk Forge OSS) for the zip file containing the Inventor model. 
        /// </summary>
        public string ZipObjectKey { get; set; }

        /// <summary>
        /// Object key (Autodesk Forge OSS) for the stp file that is exported from this configuration. 
        /// </summary>
        public string StpObjectKey { get; set; }

        /// <summary>
        /// Date and time that the record was created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }
    }
}
