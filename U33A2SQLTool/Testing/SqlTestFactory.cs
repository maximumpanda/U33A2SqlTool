using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.Types;
using PandaTester;
using U33A2SQLTool.Models.Tables;
using U33A2SQLTool.SQL.BaseTypes;
using U33A2SQLTool.SQL.Statements;
using U33A2SQLTool.SQL_Tools;

namespace U33A2SQLTool.Testing {
    internal class SqlTestFactory : ITestFactory {
        private readonly SqlManager _sqlManager;
        public List<ITest> Tests { get; set; } = new List<ITest>();
        public SqlTestFactory(SqlManager sqlManager) {
            _sqlManager = sqlManager;
            Tests.Add(TestClients());
            Tests.Add(TestPackagePickup());
            Tests.Add(TestPaidInvoice());
            Tests.Add(DisplayClientsInvoice());
            Tests.Add(TestUnpaidInvoices());
            Tests.Add(DisplayTotalUnapidInvoices());
            Tests.Add(DisplayTotalPaidInvoices());
            Tests.Add(DisplayPassedDueInvoices());
            Tests.Add(DisplayNumberofPackagesByCountry());
            Tests.Add(DisplayAboveAverageDeliveryCountries());
            Tests.Add(DisplayNumberOfTransactionsByCustomer());
        }
        private SqlTest DisplayAboveAverageDeliveryCountries() {
            SqlTest test = new SqlTest(_sqlManager) {
                Name = "Display Above Average Destinations",
                Statement =
                    @"SELECT country, count FROM 
    (SELECT e.Name AS 'Country', COUNT(a.Id) AS 'Count' 
        From Invoices a
		JOIN Packages b ON (a.Package = b.Id)
		JOIN Addresses c ON (b.Address = c.Id)
	    JOIN Cities d ON (c.City = d.Id)
        JOIN Countries e ON (d.Country = e.Id)
        Group By e.Name
    ) tb1
    CROSS JOIN
	(SELECT Avg(count) AS av
		FROM (SELECT Count(a.id) AS count
	        FROM Invoices a
			JOIN Packages b ON (a.Package = b.Id)
			JOIN Addresses c ON (b.Address = c.Id)
            JOIN Cities d ON (c.City = d.Id)
		    JOIN Countries e ON (d.Country = e.Id)
            Group BY e.Name
		) tb2
	) tb3
WHERE count > av
ORDER BY Country;",
                ExpectedFields = new[] {"Id", "Country", "Count"}
            };

            test.AddPreTestAction(a => a.ExpectedRecords = a.SqlManager.RowCount(a.Statement));

            return test;
        }
        private SqlTest DisplayClientsInvoice() {
            SqlTest test = new SqlTest(_sqlManager) {
                Name = "Get Invoice of Client",
                Statement = "Select concat(concat(a.LastName, ', '), a.FirstName) AS 'Client Name', " +
                            "g.Recipient, b.StreetAddress AS 'Street Address', c.Name AS 'City', d.Name AS 'Country', e.Name AS 'Region', " +
                            "g.PickupTime AS 'Pickup Time', g.DeliveryTime As 'Est. Delivery Time', f.Price, h.Method AS 'Payment Method'\n" +
                            "    FROM Invoices f\n" +
                            "    JOIN Packages g ON (f.Package = g.Id)\n" +
                            "    JOIN Clients a ON (g.Client = a.Id)\n" +
                            "    JOIN Addresses b ON(b.Id = g.Address)\n" +
                            "    JOIN Cities c ON (c.Id = b.City)\n" +
                            "    JOIN Countries d ON (d.Id = c.Country)\n" +
                            "    JOIN Regions e ON (e.Id = d.Region)\n" +
                            "    JOIN PaymentMethods h ON (f.PaymentMethod = h.Id)\n" +
                            "    Where g.Client = 1"
            };
            test.AddPreTestAction(
                a => {
                    a.ExpectedRecords =
                        _sqlManager.RowCount(
                            "Select * from invoices a JOIN Packages b ON (a.Package = b.Id) WHERE b.client = 1");
                });
            test.AddPreTestAction(a => {
                a.ExpectedFields = new[] {
                    "Id", "Client Name",
                    "Recipient", "Street Addresss", "City", "Country", "Region",
                    "Pickup Time", "Est. Delivery Time", "Price", "Payment Method"
                };
            });
            return test;
        }
        private SqlTest DisplayNumberofPackagesByCountry() {
            SqlTest test = new SqlTest(_sqlManager) {
                Name = "Display Number of Packages By Country",
                Statement = "SELECT e.Name AS 'Country', COUNT(a.Id) AS 'Count' \n" +
                            "    From Invoices a\n" +
                            "    JOIN Packages b ON (a.Package = b.Id)\n" +
                            "    JOIN Addresses c ON (b.Address = c.Id)\n" +
                            "    JOIN Cities d ON (c.City = d.Id)\n" +
                            "    JOIN Countries e ON (d.Country = e.Id)\n" +
                            "    Group By e.Name\n" +
                            "    ORDER BY e.Name;",
                ExpectedFields = new[] {"Id", "Country", "Count"}
            };
            test.AddPreTestAction(a => { a.ExpectedRecords = a.SqlManager.RowCount(a.Statement); });
            test.AddPreTestAction(a => {
                int count = _sqlManager.RowCount<Package>();
                a.AddExpectedValue("total Packages", count.ToString());
            });
            test.AddCondition(a => {
                long count = _sqlManager.RowCount<Package>();
                long resultCount = a.Result.Rows.Sum(x => Convert.ToUInt32(x.Fields[2].Value));
                return resultCount != count;
            }, "Package Count Mismatch");
            return test;
        }
        private SqlTest DisplayNumberOfTransactionsByCustomer() {
            SqlTest test = new SqlTest(_sqlManager) {
                Name = "Display transactions by Customer",
                Statement =
                    @"Select c.Id, concat(concat(c.LastName,', '), c.FirstName) as Client, Count(a.Id) as Count
    From clients c
    left Join Packages b ON (b.Client = c.Id)
    left Join Invoices a ON (a.Package = b.Id)
    group by c.Id",
                ExpectedFields = new[] {"Id", "Client", "Invoice Count"}
            };
            test.AddPreTestAction(a => a.ExpectedRecords = a.SqlManager.RowCount<Client>());
            return test;
        }
        private SqlTest DisplayPassedDueInvoices() {
            return DisplayTotalUnapidInvoices();
        }
        private SqlTest DisplayTotalPaidInvoices() {
            int month = DateTime.Today.Month;
            int year = DateTime.Today.Year;
            SqlTest test = new SqlTest(_sqlManager) {
                Name = "Display All Paid Invoices",
                ExpectedFields = new[] {"Id", "Package", "Date", "Price", "Paid", "Payment Method"},
                Statement = "SELECT a.Package, a.Date, a.Price, a.IsPaid AS 'Paid', b.Method AS 'Payment Method'\n " +
                            "    FROM Invoices a\n " +
                            "    JOIN PaymentMethods b ON (a.PaymentMethod = b.Id)\n " +
                            $"    WHERE MONTH(Date) = {month} AND YEAR(Date) = {year} AND IsPaid = 1"
            };
            test.AddPreTestAction(
                a => { a.ExpectedRecords = a.SqlManager.RowCount("Select * from Invoices WHERE IsPaid = 1;"); });
            return test;
        }
        private SqlTest DisplayTotalUnapidInvoices() {
            DateTime today = DateTime.Today;
            DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            SqlType<DateTime> endofCycle = new SqlType<DateTime>("") {Value = endOfMonth};
            SqlTest test = new SqlTest(_sqlManager) {
                Name = "Display All Unpaid Invoices",
                Statement = "SELECT Id, Package as 'Package Id', Date, Price, IsPaid AS 'Paid'\n" +
                            "    FROM Invoices\n" +
                            "    WHERE isPaid = 0\n" +
                            $"    AND Date < {endofCycle.FormatValue()}" +
                            "    ORDER BY Id;"
            };
            test.AddPreTestAction(
                a => {
                    a.ExpectedRecords =
                        a.SqlManager.RowCount(
                            $"Select * from Invoices where ispaid = 0 AND date < {endofCycle.FormatValue()};");
                });
            test.AddPreTestAction(a => { a.ExpectedFields = new[] {"Id", "Package", "Date", "Price", "Paid"}; });
            return test;
        }
        public SqlTest TestClients() {
            SqlTest test = new SqlTest(_sqlManager) {
                Name = "Get Clients List",
                Statement =
                    "SELECT concat(concat(a.LastName, ', '), a.FirstName) AS 'Client Name', b.StreetAddress AS 'Street Address', c.Name AS 'City', " +
                    "d.Name AS 'Country', e.Name AS 'Region', a.Telephone, a.IsBusinessAccount AS 'Account Type' \n" +
                    "    FROM Clients a\n" +
                    "    JOIN Addresses b ON (b.Id = a.Address)\n" +
                    "    JOIN Cities c ON (c.Id = b.City)\n" +
                    "    JOIN Countries d ON (d.Id = c.Country)\n" +
                    "    JOIN Regions e ON (e.Id = d.Region)\n"
            };
            //sqlTest parameters
            test.AddPreTestAction(a => { a.ExpectedRecords = _sqlManager.RowCount<Client>(); });
            test.AddPreTestAction(a => {
                a.ExpectedFields = new[] {
                    "Id", "Client Name",
                    "Street Address", "City", "Country", "Region",
                    "Telephone", "Account Type"
                };
            });
            // run the sqlTest (if no condition passed will log the result)
            return test;
        }
        public SqlTest TestPackagePickup() {
            SqlTest test = new SqlTest(_sqlManager) {
                Name = "Get today's Package Pickup List",
                Statement =
                    "SELECT f.id AS 'Package ID', f.PickupTime AS 'Pickup Time', g.Name AS 'Handler', a.FirstName AS 'First Name', a.LastName AS 'Last Name', " +
                    "b.StreetAddress AS 'Street Address', c.Name AS 'City', d.Name AS 'Country', e.Name AS 'Region' \n" +
                    "    FROM Packages f\n" +
                    "    JOIN Clients a ON (f.Client = a.Id)\n" +
                    "    JOIN Addresses b ON (b.Id = a.Address)\n" +
                    "    JOIN Cities c ON (c.Id = b.City)\n" +
                    "    JOIN Countries d ON (d.Id = c.Country)\n" +
                    "    JOIN Regions e ON (e.Id = d.Region)\n" +
                    "    JOIN Handlers g ON (f.Handler = g.Id)\n" +
                    "    ORDER BY e.Name"
            };
            SqlType<MySqlDateTime> today = new SqlType<MySqlDateTime> {Value = DateTime.Today};
            string rowSelect =
                new Statement().Select().From("Packages").Where("Date(PickupTime)", "=", $"Date({today.FormatValue()})") +
                ";";
            test.AddPreTestAction(a => { a.ExpectedRecords = _sqlManager.RowCount(rowSelect); });
            test.AddPreTestAction(a => {
                a.ExpectedFields = new[] {
                    "Id", "First Name", "Last Name",
                    "Street Address", "City", "Country", "Region",
                    "PackageID", "Handler", "Pickup Time"
                };
            });
            return test;
        }
        private SqlTest TestPaidInvoice() {
            SqlTest test = new SqlTest(_sqlManager) {
                Name = "Get Paid Invoice"
            };
            test.AddPreTestAction(a => {
                SqlCollection invoices = _sqlManager.Select("Invoices WHERE IsPaid = 1;");
                uint paidInvoiceId = (uint) invoices[0].Id.Value;
                a.Statement = "Select concat(concat(a.LastName, ', '), a.FirstName) AS 'Client Name', " +
                              "g.Recipient, b.StreetAddress AS 'Street Address', c.Name AS 'City', d.Name AS 'Country', e.Name AS 'Region', " +
                              "g.PickupTime AS 'Pickup Time', g.DeliveryTime As 'Est. Delivery Time', f.Price, h.Method AS 'Payment Method' \n" +
                              "    FROM Invoices f\n" +
                              "    JOIN Packages g ON (f.Package = g.Id)\n" +
                              "    JOIN Clients a ON (g.Client = a.Id)\n" +
                              "    JOIN Addresses b ON(b.Id = g.Address)\n" +
                              "    JOIN Cities c ON (c.Id = b.City)\n" +
                              "    JOIN Countries d ON (d.Id = c.Country)\n" +
                              "    JOIN Regions e ON (e.Id = d.Region)\n" +
                              "    JOIN PaymentMethods h ON (f.PaymentMethod = h.Id)\n" +
                              $"   Where f.Id = {paidInvoiceId};";
            });
            test.AddPreTestAction(a => { a.ExpectedRecords = 1; });
            test.AddPreTestAction(a => {
                a.ExpectedFields = new[] {
                    "Id", "Client Name",
                    "Recipient", "Street Addresss", "City", "Country", "Region",
                    "Pickup Time", "Est. Delivery Time", "Price", "Payment Method"
                };
            });
            return test;
        }
        private SqlTest TestUnpaidInvoices() {
            SqlTest test = new SqlTest(_sqlManager) {
                Name = "List Unpaid Invoices",
                Statement =
                    "SELECT a.Id AS 'Invoice Id', a.IsPaid AS 'Paid', concat(concat(b.LastName, ', '), b.FirstName) AS 'Client'\n" +
                    "FROM Invoices a\n" +
                    "JOIN Packages c ON (a.Package = c.Id)\n" +
                    "JOIN Clients b on (c.Client = b.Id)\n" +
                    "WHERE a.IsPaid = 0\n" +
                    "ORDER BY a.Id;"
            };
            test.AddPreTestAction(
                a => { a.ExpectedRecords = a.SqlManager.RowCount("Select * FROM Invoices Where invoices.isPaid = 0;"); });
            test.AddPreTestAction(a => {
                a.ExpectedFields = new[] {
                    "Id", "Invoice Id", "Paid", "Client"
                };
            });
            return test;
        }
    }
}