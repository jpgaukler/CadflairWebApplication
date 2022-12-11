using System;
using System.Collections.Generic;
using System.Text;

namespace CadflairDataAccess.Models
{
    public class ProductQuoteRequest
    {
        public int Id { get; set; } 
        public int ProductConfigurationId { get; set; } 
        public string FirstName { get; set; } 
        public string LastName { get; set; } 


        //HOW SHOULD I HANDLE EMAIL ADDRESSES AND PHONE NUMBERS FOR THIS???

        //[EmailAddress]      NVARCHAR (100) NOT NULL,
        //[PhoneNumber]       NVARCHAR (25)  NULL,
        //[PhoneExtension]    NVARCHAR (25)  NULL,



        public DateTimeOffset CreatedOn { get; set; } 
    }
}
