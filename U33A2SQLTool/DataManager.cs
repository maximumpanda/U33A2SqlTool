using PandaTester;
using U33A2SQLTool.Models.Tables;
using U33A2SQLTool.SQL_Tools;
using U33A2SQLTool.Testing;

namespace U33A2SQLTool {
    public class DataManager {
        public static string ConnectionString;
        private readonly SqlManager _sqlManager;

        public DataManager(string connectionString, string database, bool testMode = false) {
            ConnectionString = connectionString;
            _sqlManager = new SqlManager(ConnectionString, database);
            _sqlManager.RunNonQuery(SqlStatementBuilder.BuildCreateDatabase(database));
            _sqlManager.UseDatabase(database);
            RemoveTables();
            BuildTables();
            if (!testMode) return;
            SqlSeeder seeder = new SqlSeeder(_sqlManager);
            SqlTestFactory testFactory = new SqlTestFactory(_sqlManager);
            Tester tester = new Tester(seeder, testFactory);
            tester.RunAllTests();
        }

        public void BuildTables() {
            _sqlManager.AddTable(new Region());
            _sqlManager.AddTable(new Country());
            _sqlManager.AddTable(new City());
            _sqlManager.AddTable(new Address());
            _sqlManager.AddTable(new Status());
            _sqlManager.AddTable(new Handler());
            _sqlManager.AddTable(new PaymentMethod());
            _sqlManager.AddTable(new Client());
            _sqlManager.AddTable(new Authentication());
            _sqlManager.AddTable(new Package());
            _sqlManager.AddTable(new Invoice());
        }

        public void RemoveTables() {
            _sqlManager.RemoveTable(typeof(Invoice));
            _sqlManager.RemoveTable(typeof(Package));
            _sqlManager.RemoveTable(typeof(Authentication));
            _sqlManager.RemoveTable(typeof(Client));
            _sqlManager.RemoveTable(typeof(PaymentMethod));
            _sqlManager.RemoveTable(typeof(Handler));
            _sqlManager.RemoveTable(typeof(Status));
            _sqlManager.RemoveTable(typeof(Address));
            _sqlManager.RemoveTable(typeof(City));
            _sqlManager.RemoveTable(typeof(Country));
            _sqlManager.RemoveTable(typeof(Region));
        }
    }
}