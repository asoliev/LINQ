using System;
using System.Collections.Generic;
using System.Linq;
using Task1.DoNotChange;

namespace Task1
{
    public static class LinqTask
    {
        //Select the customers whose total turnover (the sum of all orders) exceeds a certain value.
        public static IEnumerable<Customer> Linq1(
            IEnumerable<Customer> customers, decimal limit)
        {
            var query = 
                from customer in customers
                where customer.Orders.Sum(t => t.Total) > limit
                select customer;

            return query;
        }

        //For each customer make a list of suppliers located in the same country and the same city.
        //Compose queries with and without grouping.
        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return customers.Select(c => (c, suppliers.Where(s => s.Country == c.Country && s.City == c.City)));
        }

        //For each customer make a list of suppliers located in the same country and the same city.
        //Compose queries with and without grouping.
        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            var suppliersByCity = suppliers.GroupBy(x => x.City);

            return customers
                .Select(
                    x => (
                        x,
                        suppliersByCity
                            .Where(sc => sc.Key == x.City)
                            .SelectMany(sc => sc)
                    )
                );
        }

        //Find all customers with the sum of all orders that exceed a certain value.
        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            return customers
                .Where(
                    c => c.Orders
                    .Any(
                        t => t.Total > limit
                    )
                );
        }

        //Select the clients, including the date of their first order.
        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            return customers
                .Where(c => c.Orders.Length > 0)
                .Select(
                    x => (
                        x,
                        x.Orders.Min(c => c.OrderDate)
                    )
                );
        }

        //Repeat the previous query but order the result by year, month,
        //turnover (descending) and customer name.
        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            return Linq4(customers)
                .OrderBy(x => x.dateOfEntry)
                .ThenBy(x => x.customer.Orders.Count())
                .ThenBy(x => x.customer.CompanyName);
        }

        /* Select the clients which either have:
        a.non-digit postal code
        b.undefined region
        c.operator code in the phone is not specified (does not contain parentheses) */
        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            return customers
                .Where(
                    x => !x.PostalCode.All(char.IsDigit)
                    || string.IsNullOrEmpty(x.Region)
                    || !x.Phone.Contains('(')
                );
        }

        //Group the products by category, then by availability in stock with ordering by cost.
        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            /* example of Linq7result
             category - Beverages
	            UnitsInStock - 39
		            price - 18.0000
		            price - 19.0000
	            UnitsInStock - 17
		            price - 18.0000
		            price - 19.0000
             */

            return products
                .GroupBy(x => x.Category)
                .Select(productsByCategory => new Linq7CategoryGroup()
                {
                    Category = productsByCategory.Key,
                    UnitsInStockGroup = productsByCategory
                    .GroupBy(p => p.UnitsInStock)
                    .Select(productsByUnitsInStock => new Linq7UnitsInStockGroup()
                    {
                        UnitsInStock = productsByUnitsInStock.Key,
                        Prices = productsByUnitsInStock.Select(p => p.UnitPrice),
                    }),
                });
        }

        /* Group the products by “cheap”, “average” and “expensive” following the rules:
        a. From 0 to cheap inclusive
        b. From cheap exclusive to average inclusive
        c. From average exclusive to expensive inclusive */
        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            return products
                .OrderBy(p => p.UnitPrice)
                .GroupBy(p => p.UnitPrice <= cheap ? cheap : p.UnitPrice <= middle ? middle : expensive)
                .Select(m => (m.Key, m.Select(x => x)));
        }

        //Calculate the average profitability of each city (average amount of orders per customer)
        //and average rate (average number of orders per customer from each city).
        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            return customers
                .GroupBy(x => x.City)
                .Select(
                    customersByCity => (
                        customersByCity.Key,
                        (int)customersByCity.Average(c => c.Orders.Sum(o => o.Total)),
                        (int)customersByCity.Average(c => c.Orders.Count())
                    )
                );
        }

        //Build a string of unique supplier country names,
        //sorted first by length and then by country.
        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            return string.Concat(
                    suppliers.Select(x => x.Country)
                    .Distinct()
                    .OrderBy(x => x.Length)
                    .ThenBy(x => x)
                );
        }
    }
}