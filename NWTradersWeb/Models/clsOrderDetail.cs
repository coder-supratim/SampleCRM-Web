using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NWTradersWeb.Models
{


    public partial class Order_Detail
    {

        public decimal Total
        {
            get
            { return ((UnitPrice - (decimal)Discount) * Quantity); }
        }
    }

}