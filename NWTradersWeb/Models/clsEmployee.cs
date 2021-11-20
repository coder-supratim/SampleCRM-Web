using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NWTradersWeb.Models
{

    /// Annotations MetaData and Decorations for EmpCode.
    public class EmployeeMetadata
    {

        /// <summary>
        /// User Name
        /// Required, Minimum - 5 characters
        /// </summary>
        [Display(Name = "Employee ID")]
        [Required(ErrorMessage = "The Employee ID is required")]
        [StringLength(5, MinimumLength = 3, ErrorMessage = "Employee ID must have atleast 3 characters")]
        public string EmployeeID;

        /// <summary>
        /// Company Name- 
        /// Required, Minimum - 5 characters
        /// </summary>
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "The Last Name is required")]
        [StringLength(20)]
        public string LastName;

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "The First Name is required")]
        [StringLength(20)]
        public string FirstName;

    }

    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee
    {

        public Order theCurrentOrder;


    }

}