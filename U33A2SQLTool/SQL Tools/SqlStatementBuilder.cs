using System.Collections.Generic;
using U33A2SQLTool.SQL.BaseTypes;
using U33A2SQLTool.SQL.Statements;

namespace U33A2SQLTool.SQL_Tools {
    public static class SqlStatementBuilder {
        public static string BuildCreateDatabase(string name) {
            return new Statement().CreateDatabase(name) + ";";
        }
        public static string BuildCreateTable(string name, SqlObject model) {
            string output = new Statement().CreateTable(model) + ";";
            //$"CREATE TABLE IF NOT EXISTS {name} ({model}\n)";
            return output;
        }
        public static string BuildDropTable(string name) {
            string output = new Statement().DropTable(name) + ";"; //$"DROP TABLE IF EXISTS {name}";
            return output;
        }
        public static string BuildInsert(string table, string field, string value) {
            string output = $"INSERT INTO {table}({field}) VALUES (\"{value}\")";
            return output;
        }
        public static string BuildInsert(List<string> tables, List<string> fields, List<object> values) {
            string output =
                $"INSERT INTO {SqlFormatter.FormatReferences(tables)}({SqlFormatter.FormatReferences(fields)}) VALUES ({SqlFormatter.FormatValues(values)})";
            return output;
        }
        public static string BuildSelect(string field, string table, string property, string oper, string value) {
            string output = new Statement().Select(field).From(table).Where(property, oper, value) + ";";
            //$"SELECT {field} From {table} WHERE {sourceTable}.{sourceProperty}=\"{value}\"";
            return output;
        }
        public static string BuildWhere(List<string> fields, List<string> values, List<string> tables) {
            string output = "";
            if (fields.Count != values.Count || values.Count != tables.Count) return null;
            output += "Where ";
            for (int i = 0; i < fields.Count; i++) {
                BuildWhere(fields[i], values[i], tables[i], true);
                if (i + 1 < fields.Count) output += "AND";
            }
            return output;
        }
        public static string BuildWhere(string field, string value, string table, bool compound = false) {
            string output = "";
            if (!compound) output += "WHERE ";
            output += $"{table}.{field} == {value} ";
            Logger.Report(output);
            return output;
        }
    }
}