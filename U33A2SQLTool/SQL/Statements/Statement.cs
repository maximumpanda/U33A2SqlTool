using System.Collections.Generic;
using System.Linq;
using U33A2SQLTool.SQL.BaseTypes;
using U33A2SQLTool.SQL_Tools;

namespace U33A2SQLTool.SQL.Statements {
    internal class Statement {
        public Statement Next { get; set; }
        public Statement Previous { get; set; }

        public string Type { get; set; }
        public string Value { get; set; }

        public Statement() {
        }
        public Statement(string type) {
            Type = type;
        }
        public Statement(Statement previous) {
            Previous = previous;
        }
        public Statement(string type, string value) {
            Type = type;
            Value = value;
        }
        public Statement(string type, params string[] values) {
            Type = type;
            Value = string.Join(", ", values);
        }

        public Statement And(string field, string oper, string value) {
            Type = Operators.And;
            Value = $"{field} {oper} {value}";
            Next = new Statement(this);
            return Next;
        }
        public Statement And(ISqlType field, string oper, string value) {
            And(field.FullName, oper, value);
            return Next;
        }
        public Statement And(ISqlType field, string oper, ISqlType otherField) {
            And(field.FullName, oper, otherField.FormatValue());
            return Next;
        }
        public Statement CreateDatabase(string name) {
            Type = Types.CreateDatabase;
            Value = name;
            Next = new Statement(this);
            return Next;
        }
        public Statement CreateTable(SqlObject table) {
            Type = Types.CreateTable;
            Value = $"{SqlHelper.Pluralize(table.Name)} ({table.BuldDefinition()})";
            Next = new Statement(this);
            return Next;
        }
        public Statement DeleteFrom(string table) {
            Type = Types.Delete;
            Value = table;
            Next = new Statement(this);
            return Next;
        }
        public Statement DeleteFrom(ISqlCollection table) {
            Type = Types.Delete;
            Value = table.Name;
            return Next;
        }
        public Statement DropTable(string name) {
            Type = Types.DropTable;
            Value = name;
            Next = new Statement(this);
            return Next;
        }
        public string Format() {
            string output = "";
            if (Previous?.Type != Type && Previous?.Type != Operators.And &&
                Previous?.Type != Operators.Or) output += $"{Type} ";
            output += $"{Value}";
            return output;
        }
        public Statement From(string table) {
            Type = Types.From;
            Value = table;
            Next = new Statement(this);
            return Next;
        }
        public Statement From(ISqlCollection table) {
            From(table.Name);
            return Next;
        }
        public Statement From(ISqlObject obj) {
            From(SqlHelper.Pluralize(obj.Name));
            return Next;
        }
        public Statement Generic(string value) {
            Type = "";
            Value = value;
            Next = new Statement(this);
            return Next;
        }
        public Statement Insert(string table) {
            Type = Types.Insert;
            Value = table;
            Next = new Statement(this);
            return Next;
        }
        public Statement Insert(ISqlObject obj) {
            return Insert(SqlHelper.Pluralize(obj.Name));
        }
        public Statement Insert(ISqlCollection table) {
            return Insert(table.Name);
        }
        public Statement On(string element1, string element2) {
            Type = Types.On;
            Value = $"{element1} = {element2}";
            Next = new Statement(this);
            return Next;
        }
        public Statement OrderBy(string field, string order = Operators.Ascending) {
            Type = Types.OrderBy;
            Value = $"{field} {order}";
            Next = new Statement(this);
            return Next;
        }
        public Statement OrderBy(ISqlType field, string order = Operators.Ascending) {
            OrderBy(field.FullName, order);
            return Next;
        }
        public Statement Scope(params string[] values) {
            Type = Types.Scope;
            Value = string.Join(", ", values) + ")";
            Next = new Statement(this);
            return Next;
        }
        public Statement Select() {
            Type = Types.Select;
            Value = "*";
            Next = new Statement(this);
            return Next;
        }
        public Statement Select(params string[] values) {
            Type = Types.Select;
            Value = string.Join(", ", values);
            Next = new Statement(this);
            return Next;
        }
        public Statement Select(params ISqlType[] values) {
            List<string> valuesList = values.Select(type => type.FullName).ToList();
            return Select(string.Join(", ", valuesList));
        }
        public override string ToString() {
            string output = "";
            Statement previous = Previous;
            while (previous != null) {
                output = previous.Format() + output;
                previous = previous.Previous;
                if (previous != null) output = " \n" + output;
            }
            return output;
        }
        public Statement Values(string values) {
            Type = Types.Values;
            Value = $"\n({values})";
            Next = new Statement(this);
            return Next;
        }
        public Statement Values(string values, bool multivalue) {
            Type = Types.Values;
            Value = $"\n{values}";
            Next = new Statement(this);
            return Next;
        }
        public Statement Where(string field, string oper, string value) {
            Type = Types.Where;
            Value = $"{field} {oper} {value}";
            Next = new Statement(this);
            return Next;
        }
        public Statement Where(ISqlType field, string oper, string value) {
            Where($"{field.Parent.Name}.{field.Name}", oper, value);
            return Next;
        }
        public Statement Where(ISqlType field, string oper, ISqlType otherField) {
            Where($"{field.Parent.Name}.{field.Name}", oper, otherField.FormatValue());
            return Next;
        }

        public static class Types {
            public const string Alter = "ALTER";
            public const string As = "AS";
            public const string CreateDatabase = "CREATE DATABASE IF NOT EXISTS";
            public const string CreateTable = "CREATE TABLE IF NOT EXISTS";
            public const string Delete = "DELETE FROM";
            public const string DropDatabase = "DROP DATABASE IF EXISTS";
            public const string DropTable = "DROP TABLE IF EXISTS";
            public const string From = "FROM";
            public const string Inner = "INNER";
            public const string Insert = "INSERT INTO";
            public const string Join = "JOIN";
            public const string On = "ON";
            public const string OrderBy = "ORDER BY";
            public const string Scope = "(";
            public const string Select = "SELECT";
            public const string Set = "SET";
            public const string Update = "UPDATE";
            public const string Values = "VALUES";
            public const string Where = "WHERE";
        }

        public static class Operators {
            public const string All = "*";
            public const string And = "AND";
            public const string Ascending = "ASC";
            public const string Decending = "DES";
            public const string Like = "LIKE";
            public const string Or = "OR";
        }
    }
}