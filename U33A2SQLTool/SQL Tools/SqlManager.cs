using System;
using U33A2SQLTool.SQL.BaseTypes;
using U33A2SQLTool.SQL.Statements;

namespace U33A2SQLTool.SQL_Tools {
    public class SqlManager {
        public SqlDatabaseModel DatabaseModel;
        public bool TestMode;
        private string _connectionString;

        public SqlManager(string connectionString, string databaseName) {
            _connectionString = connectionString;
            DatabaseModel = new SqlDatabaseModel(databaseName);
        }
        public void AddTable(SqlObject type) {
            string name = SqlHelper.Pluralize(type.Name);
            AddTable(name, type);
        }
        public void AddTable(string name, SqlObject type) {
            RunNonQuery(SqlStatementBuilder.BuildCreateTable(name, type));
            DatabaseModel.Tables.Add(type);
        }
        public void Insert(string statement) {
            RunNonQuery(statement);
        }
        public void Insert(SqlObject tableModel, SqlObject records) {
            string select =
                new Statement().Insert(tableModel).Scope(tableModel.FormatNames()).Values(records.FormatValues()) + ";";
            RunNonQuery(select);
        }
        public void Insert(SqlObject record) {
            string select = new Statement().Insert(record).Scope(record.FormatNames()).Values(record.FormatValues()) +
                            ";";
            RunNonQuery(select);
        }
        public void RemoveTable(string name) {
            RunNonQuery(SqlStatementBuilder.BuildDropTable(name));
        }
        public void RemoveTable(Type type) {
            string name = SqlHelper.Pluralize(type.Name);
            RemoveTable(name);
        }
        public int RowCount<T>() where T : SqlObject {
            T model = Activator.CreateInstance<T>();
            string select = new Statement().Select().From(model) + ";";
            SqlCollection result = RunQuery(select);
            return result.Rows.Count > 0 ? result.Rows.Count : -1;
        }
        public int RowCount(string select) {
            SqlCollection result = RunQuery(select);
            return result.Rows.Count > 0 ? result.Rows.Count : -1;
        }
        public void RunNonQuery(string command) {
            Logger.Report(command);
            if (TestMode) return;
            SqlQuery query = new SqlQuery(_connectionString);
            query.RunNonQuery(command);
        }
        public SqlCollection RunQuery(string command) {
            Logger.Report(command);
            if (TestMode) return null;
            SqlQuery query = new SqlQuery(_connectionString);
            SqlCollection results = query.RunQuery(command);
            return results;
        }
        public SqlCollection Select(SqlObject tableModel, params string[] fields) {
            string select = new Statement().Select(string.Join(", ", fields)).From(tableModel.Name) + ";";
            return RunQuery(select);
        }
        public SqlCollection Select<T>() where T : SqlObject {
            T model = Activator.CreateInstance<T>();
            string select = new Statement().Select("*").From(SqlHelper.Pluralize(model.Name)) + ";";
            return RunQuery(select);
        }
        public SqlCollection Select(string table) {
            string select = new Statement().Select().From(table) + ";";
            return RunQuery(select);
        }
        public T Select<T>(int id) where T : SqlObject {
            T model = Activator.CreateInstance<T>();
            string select = new Statement().Select().From(model.Name).Where(model["Id"], "=", id.ToString()) + ";";
            return (T) RunQuery(select)[0];
        }
        public SqlCollection Select<T>(string field, string value) where T : SqlObject {
            T model = Activator.CreateInstance<T>();
            string select =
                new Statement().Select("*").From(SqlHelper.Pluralize(model.Name)).Where(model[field].Name, "=", value) +
                ";";
            return RunQuery(select);
        }
        public void SetTestMode(bool shouldTestMode) {
            TestMode = shouldTestMode;
            Logger.SetOutputFile(TestMode ? "SqlTest.txt" : "");
        }
        public bool TableExists(Type type) {
            string select = SqlStatementBuilder.BuildSelect("table_name", "Information_Schema.Tables", "tables",
                "table_name", SqlHelper.Pluralize(type.Name));
            SqlCollection result = RunQuery(select);
            return result.Rows.Count > 0;
        }
        public void UseDatabase(string database) {
            int index = _connectionString.IndexOf("database=", StringComparison.Ordinal);
            string newConString = "";
            if (index != -1) newConString = _connectionString.Substring(0, index);
            newConString += "database=" + database;
            _connectionString = newConString;
        }
    }
}