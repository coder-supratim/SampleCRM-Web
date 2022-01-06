using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NWTradersWeb.Models
{

    /// Annotations MetaData and Decorations for EmpCode.
    public class SupplierMetadata
    {

       

        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "The Company Name is required")]
        [StringLength(40)]
        public string CompanyName;

    }

    [MetadataType(typeof(SupplierMetadata))]
    public partial class Supplier
    {

    }

}