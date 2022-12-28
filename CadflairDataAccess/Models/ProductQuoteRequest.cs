using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class ProductQuoteRequest
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; } 

        /// <summary>
        /// Foreign key to <b>ProductConfiguration</b> table. Represents the ProductConfiguration that the ProductQuoteRequest belongs to.
        /// </summary>
        public int ProductConfigurationId { get; set; } 

        /// <summary>
        /// First name of the person that submitted the request.
        /// </summary>
        public string FirstName { get; set; } 

        /// <summary>
        /// Last name of the person that submitted the request.
        /// </summary>
        public string LastName { get; set; } 

        /// <summary>
        /// Email address of the person that submitted the request.
        /// </summary>
        public string EmailAddress { get; set; } 

        /// <summary>
        /// Phone number of the person that submitted the request.
        /// </summary>
        public string PhoneNumber { get; set; } 

        /// <summary>
        /// Phone number extension of the person that submitted the request.
        /// </summary>
        public string PhoneExtension { get; set; } 

        /// <summary>
        /// Message that was included with the request.
        /// </summary>
        public string MessageText { get; set; } 

        /// <summary>
        /// Date and time that the record was created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; } 
    }
}
