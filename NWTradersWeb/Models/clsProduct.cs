using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NWTradersWeb.Models
{

    /// Annotations MetaData and Decorations for EmpCode.
    public class ProductMetadata
    {

        [Display(Name = "Product ID")]
        [Required(ErrorMessage = "The Product ID is required")]
        public int ProductID;

        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "The Product Name is required")]
        [StringLength(20)]
        public string ProductName;

        [Display(Name = "Quantity Per Unit")]
        [Required(ErrorMessage = "Quantity Per Unit is required")]
        [StringLength(20)]
        public string QuantityPerUnit;

    }

    [MetadataType(typeof(ProductMetadata))]
    public partial class Product
    {

    }

}