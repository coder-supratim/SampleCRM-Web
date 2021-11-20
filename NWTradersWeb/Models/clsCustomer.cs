using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NWTradersWeb.Models
{

    /// Annotations MetaData and Decorations for EmpCode.
    public class CustomerMetadata
    {

        /// <summary>
        /// User Name
        /// Required, Minimum - 5 characters
        /// </summary>
        [Display(Name = "Customer ID")]
        [Required(ErrorMessage = "The Customer ID is required")]
        [StringLength(5, MinimumLength = 3, ErrorMessage = "Customer ID must have atleast 3 characters")]
        public string CustomerID;

        /// <summary>
        /// Company Name- 
        /// Required, Minimum - 5 characters
        /// </summary>
        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "The Company Name is required")]
        [StringLength(20)]
        public string CompanyName;

        

    }

    [MetadataType(typeof(CustomerMetadata))]
    public partial class Customer
    {

        public Order theCurrentOrder;


    }

}