using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NWTradersWeb.Models;

namespace NWTradersWeb.Controllers
{
    public class OrdersController : Controller
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: Orders
        public ActionResult Index()
        {
            Customer currentCustomer = Session["currentCustomer"] as Customer;
            var orders = db.Orders.Include(o => o.Customer).Include(o => o.Employee).Include(o => o.Shipper);

            if (currentCustomer != null)
            {
                 orders = db.Orders.Include(o => o.Customer).Include(o => o.Employee).Include(o => o.Shipper).Where(o => o.CustomerID.Equals(currentCustomer.CustomerID));

            }

            return View(orders.ToList());
        }

        public ActionResult Cancel()
        {
            // I need to make sure the customer is still in the session - cant add a product to an order without a customer.
            Customer currentCustomer = Session["currentCustomer"] as Customer;
            if (currentCustomer == null)
                return RedirectToAction("Login", "Customers");

            //The customer should have a current order, if not then this is the first product in the cart.
            if (currentCustomer.theCurrentOrder == null)
                return RedirectToAction("Index", "Products", new { });

            currentCustomer.theCurrentOrder = null;

            return RedirectToAction("Index", "Products", new { });

        }

        // GET: Orders
       

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName");
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName");
            ViewBag.ShipVia = new SelectList(db.Shippers, "ShipperID", "CompanyName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Order order)
        {
            if (ModelState.IsValid && isValidQunatity(order))
            {
                foreach(Order_Detail od in order.Order_Details)
                {
                    Product product = db.Products.Find(od.ProductID);
                    if (product.UnitsInStock.GetValueOrDefault() > od.Quantity)
                    {
                        product.UnitsInStock = Convert.ToInt16(product.UnitsInStock.GetValueOrDefault() - od.Quantity);
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
                db.Orders.Add(order);
                db.SaveChanges();
                Session["productPageMessage"] = "Order placed successfully with order ID: " + order.OrderID;
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", order.CustomerID);
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", order.EmployeeID);
            ViewBag.ShipVia = new SelectList(db.Shippers, "ShipperID", "CompanyName", order.ShipVia);
            return View(order);
        }

        private bool isValidQunatity(Order order)
        {
            
            foreach (Order_Detail od in order.Order_Details)
            {
                Product prod = NWTradersUtilities.getProductById(od.ProductID);
                if (prod.UnitsInStock != null && od.Quantity > prod.UnitsInStock)
                {
                    Session["productPageMessage"] = "Sorry, Not enough product available!"
                        + " Please reduce Quantity of " + prod.ProductName + "  to " + prod.UnitsInStock;
                    return false;
                }
            }
            return true;
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", order.CustomerID);
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", order.EmployeeID);
            ViewBag.ShipVia = new SelectList(db.Shippers, "ShipperID", "CompanyName", order.ShipVia);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", order.CustomerID);
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", order.EmployeeID);
            ViewBag.ShipVia = new SelectList(db.Shippers, "ShipperID", "CompanyName", order.ShipVia);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //[HttpPost, ActionName("Confirm")]
        //[ValidateAntiForgeryToken]
        //public ActionResult OrderConfirmed()
        //{
        //    Customer currentCustomer = Session["currentCustomer"] as Customer;
        //    Order order = currentCustomer.theCurrentOrder;
        //    db.Orders.Add(order);
        //    db.SaveChanges();
        //    return RedirectToAction("Overview", "Customers", new { @id = currentCustomer.CustomerID });
        //}
 

        public ActionResult Confirm()
        {
            // I need to make sure the employee is still in the session - cant add a product to an order without a employee.
            Customer currentCustomer = Session["currentCustomer"] as Customer;
            if (currentCustomer == null)
                return RedirectToAction("Login", "Customers");

            Session["productPageMessage"] = "";


            //The employee should have a current order, if not then this is the first product in the cart.
            if (currentCustomer.theCurrentOrder == null)
                return RedirectToAction("Index", "Products");

            //currentCustomer.theCurrentOrder.EmployeeID = NWTradersUtilities.orderEmployeeID;

            //int? tmp = currentEmployee.theCurrentOrder.EmployeeID;

            Create(currentCustomer.theCurrentOrder);

            currentCustomer.theCurrentOrder = null;

            return RedirectToAction("Index", "Products");

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
