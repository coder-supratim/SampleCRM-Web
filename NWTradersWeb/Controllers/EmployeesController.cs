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
    public class EmployeesController : Controller
    {
        private NorthwindEntities db = new NorthwindEntities();


        public ActionResult AddProduct(int productID)
        {

            // I need to make sure the employee is still in the session - cant add a product to an order without a employee.
            Employee currentEmployee = Session["currentEmployee"] as Employee;
            if (currentEmployee == null)
                return RedirectToAction("Login", "Employees", new { @CompanyName = "", @CustomerID = "" });

            //The employee should have a current order, if not then this is the first product in the cart.
            if (currentEmployee.theCurrentOrder == null)
                CreateCurrentOrder();

            //int? tmp_1 = currentEmployee.theCurrentOrder.EmployeeID;

            // Now we have a customer with a current order.
            //  Get the product to add to the order.
            Product productToAdd = db.Products.Find(productID);

            // And we can add this product to the Order.
            // Does any order detail contain this product in the current order?
            currentEmployee.theCurrentOrder.AddToOrder(productToAdd);

            //currentEmployee.theCurrentOrder.EmployeeID = NWTradersUtilities.orderEmployeeID;

            //int? tmp_2 = currentEmployee.theCurrentOrder.EmployeeID;

            return RedirectToAction("Index", "Products");
        }

        public ActionResult RemoveProduct(int productID)
        {

            // I need to make sure the employee is still in the session - cant add a product to an order without a employee.
            Employee currentEmployee = Session["currentEmployee"] as Employee;
            if (currentEmployee == null)
                return RedirectToAction("Login", "Employees", new { @CompanyName = "", @CustomerID = "" });

            //The customer should have a current order, if not then this is the first product in the cart.
            if (currentEmployee.theCurrentOrder == null)
                return RedirectToAction("Index", "Products");

            // Now we have a employee with a current order.
            //  Get the product to remove from the order.
            Product productToRemove = db.Products.Find(productID);

            // And we can add this product to the Order.
            currentEmployee.theCurrentOrder.RemoveFromOrder(productToRemove);

            return RedirectToAction("Index", "Products");
        }

        public void CreateCurrentOrder()
        {

            Employee currentEmployee = Session["currentEmployee"] as Employee;

            if (currentEmployee == null)
                RedirectToAction("Login", "Employees", new { @CompanyName = "", @CustomerID = "" });

            if (currentEmployee.theCurrentOrder == null)
            {
                currentEmployee.theCurrentOrder = new Order
                {

                    //CustomerID = currentEmployee.CustomerID,
                    OrderDate = DateTime.Today,
                    EmployeeID = currentEmployee.EmployeeID,

                    //OrderID = AllOrderIDs().Max() + 1,

                    // Add some defaults to the new order.
                    RequiredDate = DateTime.Today.AddDays(14),
                    ShippedDate = DateTime.Today.AddDays(7),
                    ShipAddress = currentEmployee.Address
                };
            }
        }

        // GET: Employees/Overview/5
        public ActionResult Overview(string id)
        {
            if ((Session["currentEmployee"] == null) | (string.IsNullOrEmpty(id)))
            {

                return RedirectToAction("Login", "Employees", new { @CompanyName = "", @CustomerID = "" });
            }
            else
            {

                Employee currentEmployee = db.Employees.Find(Convert.ToInt32(id));

                List<Order> currentEmployeeOrders = currentEmployee.Orders.OrderByDescending(o => o.OrderDate).ToList();

                List<Product> currentEmployeeRecentProducts = new List<Product>();
                List<Product> currentEmployeeFrequentProducts = new List<Product>();
                List<int> currentEmployeeProductIds = new List<int>();
                HashSet<int> currentEmployeeUniqueProductIds = new HashSet<int>();
                IDictionary<int, int> ProductIdsFrequency = new Dictionary<int, int>();

                //numberNames.Add(2, "Two");

                foreach (Order order in currentEmployeeOrders)
                {
                    foreach (Order_Detail order_detail in order.Order_Details)
                    {
                        currentEmployeeProductIds.Add(order_detail.ProductID);
                        Product product = db.Products.Find(order_detail.ProductID);
                        currentEmployeeRecentProducts.Add(product);
                    }
                }

                foreach (int product_id in currentEmployeeProductIds)
                {
                    currentEmployeeUniqueProductIds.Add(product_id);
                }

                foreach (int product_id in currentEmployeeUniqueProductIds)
                {
                    ProductIdsFrequency.Add(product_id, CountOccurences(currentEmployeeProductIds, product_id));
                }

                IEnumerable<int> sortedProductIdsFrequency = from entry in ProductIdsFrequency orderby entry.Value descending select entry.Key;

                foreach (int product_id in sortedProductIdsFrequency)
                {
                    Product product = db.Products.Find(product_id);
                    currentEmployeeFrequentProducts.Add(product);
                }

                ViewBag.currentEmployeeRecentProducts = currentEmployeeRecentProducts.Take(5).ToList();
                ViewBag.currentEmployeeFrequentProducts = currentEmployeeFrequentProducts.Take(5).ToList();
                ViewBag.currentEmployeeOrders = currentEmployeeOrders.Take(5).ToList();

                return View(currentEmployee);

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


        public ActionResult Login(string employeeLastName, string employeeID)
        {
            if (string.IsNullOrEmpty(employeeLastName))
            {
                ViewBag.employeeLastName = "Please select your last name";
                return View();
            }

            if (string.IsNullOrEmpty(employeeID))
            {
                ViewBag.employeeIDMessage = "Please enter a valid employee ID";
                return View();
            }

            Employee currentEmployee = db.Employees.Where(e => e.LastName.Equals(employeeLastName)).
                Where(e => e.EmployeeID.ToString().Equals(employeeID)).Select(e => e).FirstOrDefault();

            Session["currentEmployee"] = currentEmployee;

            if (currentEmployee == null)
            {
                ViewBag.employeeIDMessage = "The employee ID you entered is not valid. Please " +
                    "enter a valid employee ID as your password";
                return View();
            }
            else
            {
                return RedirectToAction("Overview", "Employees", new { @id = currentEmployee.EmployeeID });
            }

        }



        // GET: Employees
        public ActionResult Index(string searchLastName = "", string searchCountryName = "")
        {

            IEnumerable<Employee> theEmployees = db.Employees.
                OrderBy(e => e.LastName).
                Select(e => e).ToList();


            if (theEmployees.Count() > 0)
                // Here the ignore case allows for searches that are not case sensitive.
                // Use this to do case insensitive searches for any field.
                if (string.IsNullOrEmpty(searchLastName) == false)
                {
                    // Here the ignore case allows for searches that are not case sensitive.
                    // Use this to do case insensitive searches for any field.
                    theEmployees = theEmployees.
                        Where(e => e.LastName.StartsWith(searchLastName, ignoreCase: true, new System.Globalization.CultureInfo("en-US"))).
                        OrderBy(e => e.LastName).
                        Select(e => e);
                }
            ViewBag.searchLastName = searchLastName;


            if (theEmployees.Count() > 0)
                if (string.IsNullOrEmpty(searchCountryName) == false)
                {
                    theEmployees = theEmployees.
                        Where(e => e.Country.Equals(searchCountryName)).
                        OrderBy(e => e.LastName).
                        Select(e => e);
                }
            ViewBag.searchCountryName = searchCountryName;


            return View(theEmployees.ToList());
        }


        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.ReportsTo = new SelectList(db.Employees, "EmployeeID", "LastName");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,LastName,FirstName,Title,TitleOfCourtesy,BirthDate,HireDate,Address,City,Region,PostalCode,Country,HomePhone,Extension,Photo,Notes,ReportsTo,PhotoPath")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ReportsTo = new SelectList(db.Employees, "EmployeeID", "LastName", employee.ReportsTo);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReportsTo = new SelectList(db.Employees, "EmployeeID", "LastName", employee.ReportsTo);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,LastName,FirstName,Title,TitleOfCourtesy,BirthDate,HireDate,Address,City,Region,PostalCode,Country,HomePhone,Extension,Photo,Notes,ReportsTo,PhotoPath")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ReportsTo = new SelectList(db.Employees, "EmployeeID", "LastName", employee.ReportsTo);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
