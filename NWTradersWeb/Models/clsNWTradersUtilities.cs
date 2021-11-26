using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.Mvc;

namespace NWTradersWeb.Models
{
    public class NWTradersUtilities
    {

        private static NorthwindEntities nwEntities = new NorthwindEntities();

        public static Product getProductById(int productId)
        {
            return nwEntities.Products.Where(p => p.ProductID == productId)
                .Select(p => p)
                .FirstOrDefault();

        }

        public static List<string> AllSalesPersonnel()
        {
            List<string> allSalesPersonnel = new List<string>();
            allSalesPersonnel =
                nwEntities.
                Employees.
                OrderBy(s => s.LastName).
                Where(s => s.Title.Equals("Sales Representative")).
                Select(s => s.LastName).
                ToList();

            return allSalesPersonnel;
        }
 public static List<SelectListItem> AllSalesPersonnels(string salesPersonName)
        {
            List<SelectListItem> allSalesPersonnels = new List<SelectListItem>();

            var queryResults =
            (from e in nwEntities.Employees
             where string.Equals(e.Title, "Sales Representative")
             select new { FullName = e.FirstName + " " + e.LastName + "- EmpId: " + e.EmployeeID }).Distinct();

            foreach (var item in queryResults)
            {
                if (!string.IsNullOrEmpty(item.FullName))
                {
                    SelectListItem si = 
                        new SelectListItem
                        {
                            Text = item.FullName
                        };
                    allSalesPersonnels.Add(si);
                }
            }

            if (string.IsNullOrEmpty(salesPersonName) == false)
            {
                allSalesPersonnels.Find(c => c.Text == salesPersonName).Selected = true;
            }

            return allSalesPersonnels;
        }

        public static List<Order> FindOrderForCustomer(string customerID)
        {
            List<Order> allOrderForCustomer = 
          
                nwEntities.
                Orders.
                OrderByDescending(o => o.OrderDate).
                Where(o => o.CustomerID.Equals(customerID)).
                Select(o => o).
                ToList();

            return allOrderForCustomer;
        }

        public static List<string> AllCustomerContactNames()
        {
            List<string> allCustomerContactNames = new List<string>();
            allCustomerContactNames =
                nwEntities.
                Customers.
                OrderBy(c => c.CompanyName).
                Select(c => c.CompanyName).
                ToList();

            return allCustomerContactNames;
        }
        public static List<SelectListItem> AllCountries(string searchCountryName)
        {


            List<SelectListItem> allCountries = (from e in nwEntities.Employees.
                Select(e => e.Country).Distinct().AsEnumerable()
                                                 select new SelectListItem
                                                 {
                                                     Text = e
                                                 }).ToList();

            // pre-select an item of the list
            if (string.IsNullOrEmpty(searchCountryName) == false)
            {
                allCountries.Find(e => e.Text == searchCountryName).Selected = true;
            }


            return allCountries;

        }

        public static List<SelectListItem> AllCategories(string searchCategoryName)
        {


            List<SelectListItem> allCategories = (from c in nwEntities.Categories.AsEnumerable()
                                                  select new SelectListItem
                                                  {
                                                      Text = c.CategoryName
                                                  }).ToList();

            // pre-select an item of the list
            if (string.IsNullOrEmpty(searchCategoryName) == false)
            {
                allCategories.Find(c => c.Text == searchCategoryName).Selected = true;
            }


            return allCategories;

        }

    }
}
