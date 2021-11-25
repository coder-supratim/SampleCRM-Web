﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NWTradersWeb.Models
{


    public partial class Order
    {


        public void AddToOrder(Product productToAdd)
        {

            Order_Detail odWithProduct = null;

            odWithProduct = this.Order_Details.
                Where(od => od.ProductID == productToAdd.ProductID).
                Select(od => od).
                FirstOrDefault();

            if (odWithProduct == null)
            {
                // If the order detail is not found, then it doesnt exist in the database - 
                // Make a new order detail using the product and the current order (this)
                odWithProduct = new Order_Detail
                {
                    ProductID = productToAdd.ProductID,
                    

                    Order = this,
                    OrderID = this.OrderID,

                    Discount = 0F,

                    UnitPrice = productToAdd.UnitPrice.Value,
                    Quantity = 1
                };

                // Add the new order detail to the current order.
                this.Order_Details.Add(odWithProduct);

            }
            // Otherwise, increment quantity.
            else
            {
                odWithProduct.Quantity++;
            }
            return;
        }

        public void RemoveFromOrder(Product productToRemove)
        {

            Order_Detail odWithProduct = null;

            odWithProduct = this.Order_Details.
                Where(od => od.ProductID == productToRemove.ProductID).
                Select(od => od).
                FirstOrDefault();

            if (odWithProduct != null)
            // Otherwise, decrement quantity.
            {
                odWithProduct.Quantity--;

                if (odWithProduct.Quantity == 0)
                    this.Order_Details.Remove(odWithProduct);
            }

            return;

        }


        public decimal orderTotal
        {
            get
            { return this.Order_Details.Sum(od => ((od.UnitPrice - (decimal)od.Discount) * od.Quantity)); }
        }

    }

}