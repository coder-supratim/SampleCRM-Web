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
    public class CustomersController : Controller
    {
        private NorthwindEntities db = new NorthwindEntities();



        public ActionResult AddProduct(int productID)
        {

            // I need to make sure the employee is still in the session - cant add a product to an order without a employee.
            Customer currentCustomer = Session["currentCustomer"] as Customer;
            if (currentCustomer == null)
                return RedirectToAction("Login", "Customer", new { @CompanyName = "", @CustomerID = "" });

            //The employee should have a current order, if not then this is the first product in the cart.
            if (currentCustomer.theCurrentOrder == null)
                CreateCurrentOrder();

            //int? tmp_1 = currentEmployee.theCurrentOrder.EmployeeID;

            // Now we have a customer with a current order.
            //  Get the product to add to the order.
            Product productToAdd = db.Products.Find(productID);

            // And we can add this product to the Order.
            // Does any order detail contain this product in the current order?
            currentCustomer.theCurrentOrder.AddToOrder(productToAdd);

            //currentEmployee.theCurrentOrder.EmployeeID = NWTradersUtilities.orderEmployeeID;

            //int? tmp_2 = currentEmployee.theCurrentOrder.EmployeeID;

            return RedirectToAction("Index", "Products");
        }

        public ActionResult RemoveProduct(int productID)
        {

            // I need to make sure the employee is still in the session - cant add a product to an order without a employee.
            Customer currentCustomer = Session["currentCustomer"] as Customer;
            if (currentCustomer == null)
                return RedirectToAction("Login", "Customers", new { @CompanyName = "", @CustomerID = "" });

            //The customer should have a current order, if not then this is the first product in the cart.
            if (currentCustomer.theCurrentOrder == null)
                return RedirectToAction("Index", "Products");

            // Now we have a employee with a current order.
            //  Get the product to remove from the order.
            Product productToRemove = db.Products.Find(productID);

            // And we can add this product to the Order.
            currentCustomer.theCurrentOrder.RemoveFromOrder(productToRemove);

            return RedirectToAction("Index", "Products");
        }

        public void CreateCurrentOrder()
        {

            Customer currentCustomer = Session["currentCustomer"] as Customer;

            if (currentCustomer == null)
                RedirectToAction("Login", "Customers", new { @CompanyName = "", @CustomerID = "" });

            if (currentCustomer.theCurrentOrder == null)
            {
                currentCustomer.theCurrentOrder = new Order
                {

                    //CustomerID = currentEmployee.CustomerID,
                    OrderDate = DateTime.Today,
                    CustomerID = currentCustomer.CustomerID,

                    //OrderID = AllOrderIDs().Max() + 1,

                    // Add some defaults to the new order.
                    RequiredDate = DateTime.Today.AddDays(14),
                    ShippedDate = DateTime.Today.AddDays(7),
                    ShipAddress = currentCustomer.Address,
                };
            }
        }

        // GET: Customers/Overview/5
        public ActionResult Overview(string id)
        {
            if ((Session["currentCustomer"] == null) | (string.IsNullOrEmpty(id)))
            {

                return RedirectToAction("Login", "Customers", new { @CompanyName = "", @CustomerID = "" });
            }
            else
            {

                Customer currentCustomer = db.Customers.Find(id);

                List<Order> currentCustomerOrders = currentCustomer.Orders.OrderByDescending(o => o.OrderDate).ToList();
                if (null == currentCustomerOrders)
                {
                    currentCustomerOrders = NWTradersUtilities.FindOrderForCustomer(currentCustomer.CustomerID);

                }
                

                List<Product> currentCustomerRecentProducts = new List<Product>();
                List<Product> currentCustomerFrequentProducts = new List<Product>();
                List<int> currentCustomerProductIds = new List<int>();
                HashSet<int> currentCustomerUniqueProductIds = new HashSet<int>();
                IDictionary<int, int> ProductIdsFrequency = new Dictionary<int, int>();

                //numberNames.Add(2, "Two");

                foreach (Order order in currentCustomerOrders)
                {
                    foreach (Order_Detail order_detail in order.Order_Details)
                    {
                        currentCustomerProductIds.Add(order_detail.ProductID);
                        Product product = db.Products.Find(order_detail.ProductID);
                        currentCustomerRecentProducts.Add(product);
                    }
                }

                foreach (int product_id in currentCustomerProductIds)
                {
                    currentCustomerUniqueProductIds.Add(product_id);
                }

                foreach (int product_id in currentCustomerUniqueProductIds)
                {
                    ProductIdsFrequency.Add(product_id, CountOccurences(currentCustomerProductIds, product_id));
                }

                IEnumerable<int> sortedProductIdsFrequency = from entry in ProductIdsFrequency orderby entry.Value descending select entry.Key;

                foreach (int product_id in sortedProductIdsFrequency)
                {
                    Product product = db.Products.Find(product_id);
                    currentCustomerFrequentProducts.Add(product);
                }

                ViewBag.currentCustomerRecentProducts = currentCustomerRecentProducts.Take(5).ToList();
                ViewBag.currentCustomerFrequentProducts = currentCustomerFrequentProducts.Take(5).ToList();
                ViewBag.currentCustomerOrders = currentCustomerOrders.Take(5).ToList();

                return View(currentCustomer);

            }


        }

        public int CountOccurences(List<int> listNumbers, int focalNumber)
        {
            int counter = 0;
            foreach (int number in listNumbers)
            {
                if (number == focalNumber)
                    counter++;
            }
            return counter;
        }


        public ActionResult Login(string companyName, string customerID)
        {
            if (string.IsNullOrEmpty(companyName))
            {
                ViewBag.companyName = "Please select your Company Name";
                return View();
            }

            if (string.IsNullOrEmpty(customerID))
            {
                ViewBag.customerIDMessage = "Please enter a valid Custome ID";
                return View();
            }

            Customer currentCustomer = db.Customers.Where(c => c.CompanyName.Equals(companyName)).
                Where(c => c.CustomerID.ToString().Equals(customerID)).Select(e => e).FirstOrDefault();

            Session["currentCustomer"] = currentCustomer;

            if (currentCustomer == null)
            {
                ViewBag.customerIDMessage = "The Customer ID you entered is not valid. Please " +
                    "enter a valid Customer ID as your password";
                return View();
            }
            else
            {
                return RedirectToAction("Overview", "Customers", new { @id = currentCustomer.CustomerID });
            }

        }



        // GET: Customers
        public ActionResult Index(string companyName = "", string searchCountryName = "")
        {

            IEnumerable<Customer> theCustomers = db.Customers.
                OrderBy(c => c.CompanyName).
                Select(e => e).ToList();


            if (theCustomers.Count() > 0)
                // Here the ignore case allows for searches that are not case sensitive.
                // Use this to do case insensitive searches for any field.
                if (string.IsNullOrEmpty(companyName) == false)
                {
                    // Here the ignore case allows for searches that are not case sensitive.
                    // Use this to do case insensitive searches for any field.
                    theCustomers = theCustomers.
                        Where(e => e.CompanyName.StartsWith(companyName, ignoreCase: true, new System.Globalization.CultureInfo("en-US"))).
                        OrderBy(e => e.CompanyName).
                        Select(e => e);
                }
            ViewBag.CompanyName = companyName;


            if (theCustomers.Count() > 0)
                if (string.IsNullOrEmpty(searchCountryName) == false)
                {
                    theCustomers = theCustomers.
                        Where(e => e.Country.Contains(searchCountryName)).
                        OrderBy(e => e.CompanyName).
                        Select(e => e);
                }
            ViewBag.searchCountryName = searchCountryName;


            return View(theCustomers.ToList());
        }


        // GET: Customers
        //public ActionResult Index()
        //{
        //    return View(db.Customers.ToList());
        //}

        // GET: Customers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Overview", "Customers", new { @id = customer.CustomerID });
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
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
