using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class ProductVersion
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to <b>Product</b> table. Represents the Product that the ProductVersion belongs to.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// File name of the root Inventor file in the zip folder for this Product. This is needed for the Forge Model Derivative and Design Automation services.
        /// </summary>
        public string RootFileName { get; set; }

        /// <summary>
        /// Json object that contains the form definition for the ProductVersion. This is derived from the iLogic form in the Inventor model.
        /// </summary>
        public string ILogicFormJson { get; set; }

        /// <summary>
        /// Version number of the Product.
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Boolean that indicates if this Product will allow custom configurations through the form definition that is provided in the ILogicFormJson field.
        /// </summary>
        public bool IsConfigurable { get; set; }

        /// <summary>
        /// Date and time that the record was created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Foreign key to <b>User</b> table. Represents the id of the User that created the ProductVersion.
        /// </summary>
        public int CreatedById { get; set; }
    }
}
