using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NWTradersWeb.Models
{

    public class EmployeeMetadata
    {

      
       [Display(Name = "First Name")]
       [Required(ErrorMessage = "Employee First Name is required")]
        [StringLength(10)]
        public string FirstName;

      
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Employee Last Name is required")]
        [StringLength(20)]
        public string LastName;

        [Display(Name = "Employee Title")]
        [StringLength(30)]
        public string Title;

    }

    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee
    {



    }

}