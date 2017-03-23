using System;
using MySql.Data.MySqlClient;
using U33A2SQLTool.SQL.BaseTypes;

namespace U33A2SQLTool.SQL_Tools {
    public class SqlQuery {
        private readonly string _connectionString;
        public SqlQuery(string connectionString) {
            _connectionString = connectionString;
        }
        private static SqlCollection ParseQueryResult(MySqlDataReader reader) {
            SqlCollection result = new SqlCollection();
            while (reader.Read()) {
                SqlObject obj = new SqlObject(typeof(uint)) {Name = "results"};
                obj.Id.Value = result.Count + 1;
                for (int i = 0; i < reader.FieldCount; i++) {
                    SqlType newSqlType = new SqlType {
                        Type = reader.GetFieldType(i),
                        Name = reader.GetName(i),
                        Value = reader.GetValue(i)
                    };
                    obj[reader.GetName(i)] = newSqlType;
                }
                result.Add(obj);
            }
            return result;
        }

        public int RunNonQuery(string command) {
            using (MySqlConnection con = new MySqlConnection(_connectionString))
            using (MySqlCommand com = con.CreateCommand()) {
                com.CommandText = command;
                return TryExecuteNonQuery(com);
            }
        }
        public SqlCollection RunQuery(string query) {
            SqlCollection result;
            using (MySqlConnection con = new MySqlConnection(_connectionString))
            using (MySqlCommand com = con.CreateCommand()) {
                com.CommandText = query;
                result = TryExecuteQuery(com);
            }
            return result;
        }
        private static int TryExecuteNonQuery(MySqlCommand command) {
            int result;
            try {
                command.Connection.Open();
                command.Prepare();
                result = command.ExecuteNonQuery();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            finally {
                command.Connection.Close();
            }
            return result;
        }
        private static SqlCollection TryExecuteQuery(MySqlCommand command) {
            SqlCollection result;
            MySqlDataReader reader = null;
            try {
                command.Connection.Open();
                command.Prepare();
                reader = command.ExecuteReader();
                result = ParseQueryResult(reader);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            finally {
                command.Connection.Close();
                reader?.Close();
            }
            return result;
        }
    }
}