using System;
using PandaTester;
using U33A2SQLTool.Models.Tables;
using U33A2SQLTool.SQL.BaseTypes;
using U33A2SQLTool.SQL.Statements;
using U33A2SQLTool.SQL_Tools;

namespace U33A2SQLTool.Testing {
    public class SqlSeeder : ISeeder {
        public static int AddressCount;
        public static int CityCount;
        public static int ClientCount;
        public static int CountryCount;
        public static int HandlersCount;
        public static int InvoiceCount;
        public static int PackageCount;
        public static int PaymentMethodCount;
        public static int RegionCount;
        public static int StatusCount;
        private static SqlManager _sqlManager;

        public SqlSeeder(SqlManager manager) {
            _sqlManager = manager;
        }

        public void Seed() {
            SeedRegion(7);
            SeedCountries(15);
            SeedCities(15);
            SeedAddresses(30);
            SeedStatus("At Depot", "On Route", "Delivered");
            SeedHandlers(10);
            SeedPaymentMethods("Credit", "Debit", "Check", "Paypal");
            SeedClients(30);
            SeedPackages(40);
            SeedInvoices();
        }
        private static void SeedAddresses(int count) {
            SqlCollection cities = _sqlManager.Select<City>();
            string addressValues = "";
            for (int i = 0; i < count; i++) {
                Address newAddress = new Address {
                    ["StreetAddress"] = {Value = "SqlTest Street Address" + i},
                    ["City"] = {Value = cities[SqlHelper.RandomInt(cities.Count) - 1]["Id"].Value},
                    ["PostCode"] = {Value = "PC00" + i}
                };
                addressValues += $"({newAddress.FormatValues()}),\n";
                AddressCount++;
            }
            addressValues = addressValues.Remove(addressValues.Length - 2);
            string insertAddresses =
                new Statement().Insert(new Address())
                    .Scope("StreetAddress", "City", "PostCode")
                    .Values(addressValues, true)
                    .ToString();
            _sqlManager.Insert(insertAddresses);
        }
        private static void SeedCities(int count) {
            SqlCollection countries = _sqlManager.Select<Country>();
            string cityValues = "";
            for (int i = 0; i < count; i++) {
                City newCity = new City {
                    ["Name"] = {Value = "TestCity" + i},
                    ["Country"] = {Value = countries[SqlHelper.RandomInt(countries.Count) - 1]["Id"].Value}
                };
                cityValues += $"({newCity.FormatValues()}),\n";
                CityCount++;
            }
            cityValues = cityValues.Remove(cityValues.Length - 2);
            string insertCities =
                new Statement().Insert(new City()).Scope("Name", "Country").Values(cityValues, true).ToString();
            _sqlManager.Insert(insertCities);
        }
        private static void SeedClients(int count) {
            SqlCollection addresses = _sqlManager.Select<Address>();
            string clientValues = "";
            string authValues = "";
            for (int i = 0; i < count; i++) {
                Authentication newAuth = new Authentication();
                Client newClient = new Client {
                    ["FirstName"] = {Value = "SqlTest"},
                    ["LastName"] = {Value = "Client" + i},
                    ["Address"] = {Value = addresses[SqlHelper.RandomInt(addresses.Count) - 1]["Id"].Value},
                    ["IsBusinessAccount"] = {Value = SqlHelper.RandomBool()},
                    ["Telephone"] = {Value = "TestNumber"}
                };
                clientValues += $"({newClient.FormatValues()}),\n";
                newAuth["Email"].Value = $"TestClient{i}@email.co.uk";
                newAuth["PasswordHash"].Value = "FILLERHASHFILLERHASHFILLERHASH";
                newAuth["Salt"].Value = "salty";
                authValues += $"({newAuth.FormatValues()}),\n";
                ClientCount++;
            }
            clientValues = clientValues.Remove(clientValues.Length - 2);
            authValues = authValues.Remove(authValues.Length - 2);
            string insertClients =
                new Statement().Insert(new Client())
                    .Scope("FirstName", "LastName", "Address", "IsBusinessAccount", "Telephone")
                    .Values(clientValues, true)
                    .ToString();
            string insertAuths =
                new Statement().Insert(new Authentication())
                    .Scope("Email", "PasswordHash", "Salt")
                    .Values(authValues, true)
                    .ToString();
            _sqlManager.Insert(insertClients);
            _sqlManager.Insert(insertAuths);
        }
        private static void SeedCountries(int count) {
            SqlCollection regions = _sqlManager.Select<Region>();
            string countryValues = "";
            for (int i = 0; i < count; i++) {
                Country newCountry = new Country {
                    ["Name"] = {Value = "TestCountry" + i},
                    ["Region"] = {Value = regions[SqlHelper.RandomInt(regions.Count) - 1]["Id"].Value}
                };
                countryValues += $"({newCountry.FormatValues()}),\n";
                CountryCount++;
            }
            countryValues = countryValues.Remove(countryValues.Length - 2);
            string insertCountries =
                new Statement().Insert(new Country()).Scope("Name", "Region").Values(countryValues, true).ToString();
            _sqlManager.Insert(insertCountries);
        }
        private static void SeedHandlers(int count) {
            SqlCollection addresses = _sqlManager.Select<Address>();
            string handlerValues = "";
            for (int i = 0; i < count; i++) {
                Handler newHandler = new Handler {
                    ["Name"] = {Value = "TestHandler" + i},
                    ["Address"] = {Value = addresses[SqlHelper.RandomInt(addresses.Count) - 1]["Id"].Value}
                };
                handlerValues += $"({newHandler.FormatValues()}),\n";
                HandlersCount++;
            }
            handlerValues = handlerValues.Remove(handlerValues.Length - 2);
            string insertHandlers =
                new Statement().Insert(new Handler()).Scope("Name", "Address").Values(handlerValues, true).ToString();
            _sqlManager.Insert(insertHandlers);
        }
        public static void SeedInvoices() {
            const float pricePerKg = 40;
            SqlCollection packages = _sqlManager.Select<Package>();
            SqlCollection paymentMethods = _sqlManager.Select<PaymentMethod>();
            SqlCollection clients = _sqlManager.Select<Client>();
            string invoiceValues = "";
            foreach (SqlObject package in packages.Rows) {
                Invoice newInvoice = new Invoice {
                    ["Package"] = {Value = package["Id"].Value},
                    ["Date"] = {Value = package["PickupTime"].Value},
                    ["Price"] = {Value = pricePerKg * (float) package["Weight"].Value}
                };
                foreach (SqlObject client in clients.Rows) {
                    if (client.Id.FormatValue() != package["Client"].FormatValue()) continue;
                    if ((bool) client["IsBusinessAccount"].Value)
                        newInvoice["IsPaid"].Value = SqlHelper.RandomBool();
                    else newInvoice["IsPaid"].Value = true;
                    break;
                }
                if ((bool) newInvoice["IsPaid"].Value)
                    newInvoice["PaymentMethod"].Value = SqlHelper.RandomInt(paymentMethods.Count);
                else
                    newInvoice["PaymentMethod"].Value = null;
                invoiceValues += $"({newInvoice.FormatValues()}),\n";
                InvoiceCount++;
            }
            invoiceValues = invoiceValues.Remove(invoiceValues.Length - 2);
            string insertInvoices =
                new Statement().Insert(new Invoice())
                    .Scope("Package", "Date", "Price", "IsPaid", "PaymentMethod")
                    .Values(invoiceValues, true)
                    .ToString();
            _sqlManager.Insert(insertInvoices);
        }
        private static void SeedPackages(int count) {
            DateTime pickup = DateTime.Today;
            SqlCollection clients = _sqlManager.Select<Client>();
            int statusCount = _sqlManager.RowCount<Status>();
            int handlerCount = _sqlManager.RowCount<Handler>();
            string packageValues = "";
            for (int i = 0; i < count; i++) {
                SqlObject recipient = clients[SqlHelper.RandomInt(clients.Count - 1)];
                Package newPackage = new Package {
                    ["Client"] = {Value = clients[SqlHelper.RandomInt(clients.Count) - 1]["Id"].Value},
                    ["Status"] = {Value = SqlHelper.RandomInt(statusCount)},
                    ["Handler"] = {Value = SqlHelper.RandomInt(handlerCount)},
                    ["Recipient"] = {Value = (string) recipient["FirstName"].Value + recipient["LastName"].Value},
                    ["Address"] = {Value = recipient["Address"].Value},
                    ["PickupTime"] = {Value = pickup.AddHours(SqlHelper.RandomInt(23))},
                    ["DeliveryTime"] = {Value = pickup.AddDays(SqlHelper.RandomInt(5))},
                    ["Weight"] = {Value = SqlHelper.RandomFloat()}
                };
                packageValues += $"({newPackage.FormatValues()}),\n";
                PackageCount++;
            }
            packageValues = packageValues.Remove(packageValues.Length - 2);
            string insertPackages =
                new Statement().Insert(new Package())
                    .Scope("Client", "Status", "Handler", "Recipient", "Address", "PickupTime", "DeliveryTime", "Weight")
                    .Values(packageValues, true)
                    .ToString();
            _sqlManager.Insert(insertPackages);
        }
        private static void SeedPaymentMethods(params string[] values) {
            string methodValues = "";
            foreach (string value in values) {
                PaymentMethod newPaymentMethod = new PaymentMethod {["Method"] = {Value = value}};
                methodValues += $"({newPaymentMethod.FormatValues()}),\n";
                PaymentMethodCount++;
            }
            methodValues = methodValues.Remove(methodValues.Length - 2);
            string insertMethods =
                new Statement().Insert(new PaymentMethod()).Scope("Method").Values(methodValues, true).ToString();
            _sqlManager.Insert(insertMethods);
        }
        private static void SeedRegion(int count) {
            string regionValues = "";
            for (int i = 0; i < count; i++) {
                Region newRegion = new Region {["Name"] = {Value = "TestRegion" + i}};
                RegionCount++;
                regionValues += $"({newRegion.FormatValues()}),\n";
            }
            regionValues = regionValues.Remove(regionValues.Length - 2);
            string insertRegions =
                new Statement().Insert(new Region()).Scope("Name").Values(regionValues, true).ToString();
            _sqlManager.Insert(insertRegions);
        }
        private static void SeedStatus(params string[] values) {
            string statusValues = "";
            foreach (string value in values) {
                Status newStatus = new Status {["Status"] = {Value = value}};
                statusValues += $"({newStatus.FormatValues()}),\n";
                StatusCount++;
            }
            statusValues = statusValues.Remove(statusValues.Length - 2);
            string insertStatus =
                new Statement().Insert(new Status()).Scope("Status").Values(statusValues, true).ToString();
            _sqlManager.Insert(insertStatus);
        }
    }
}