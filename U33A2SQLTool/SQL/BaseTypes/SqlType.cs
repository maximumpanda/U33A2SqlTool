using System;
using MySql.Data.Types;
using U33A2SQLTool.SQL_Tools;

namespace U33A2SQLTool.SQL.BaseTypes {
    public class SqlType<T> : SqlType {
        public SqlType() {
            Type = typeof(T);
        }
        public SqlType(string name) {
            Name = name;
            Type = typeof(T);
        }
        public SqlType(string name, uint size = 0, bool foreignKey = false, bool unique = false, bool notNull = true,
            bool autoIncrement = false) {
            Name = name;
            Type = typeof(T);
            Size = size;
            ForeignKey = foreignKey;
            if (foreignKey) ReferencesTable = SqlHelper.Pluralize(Name);
            Unique = unique;
            NotNull = notNull;
            AutoIncrement = autoIncrement;
        }

        public void SetValue(T value) {
            Value = value;
        }
    }

    public class SqlType : ISqlType {
        private string _name;
        public bool AutoIncrement { get; set; }
        public bool ForeignKey { get; set; }

        public string FullName => $"{Parent.Name}.{Name}";
        public string Name {
            get { return _name; }
            set {
                _name = value;
                if (ReferencesTable != "") ReferencesTable = SqlHelper.Pluralize(value);
            }
        }
        public bool NotNull { get; set; } = true;
        public SqlObject Parent { get; set; }
        public string ReferencesTable { get; set; } = "";
        public uint Size { get; set; }
        public Type Type { get; set; }
        public bool Unique { get; set; }
        public object Value { get; set; }
        public string As(string alias) {
            return $"{Name} AS {alias}";
        }

        public string FormatDefinition() {
            return
                $"\n{Name} {SqlTypeHelper.FormatType(Type, Size)} {SqlTypeHelper.FormatTypeProperties(NotNull, AutoIncrement, Unique)},";
        }
        public string FormatReference() {
            return ReferencesTable == "" ? "" : $"\nFOREIGN KEY ({Name}) REFERENCES {ReferencesTable} (Id),";
        }

        public string FormatValue() {
            if (Type == typeof(string)) return $"\"{Value}\"";
            if (Type == typeof(MySqlDateTime) || Type == typeof(DateTime))
                return $"\'{(DateTime) Value:yyyy-MM-dd HH:mm:ss}\'";
            if (Type == typeof(float)) return $"{(float) Value:0.00}";
            if (Value == null) return "NULL";
            return $"{Convert.ChangeType(Value, Type)}";
        }
    }

    public static class SqlTypeHelper {
        public static string FormatType(Type type, uint size = 0) {
            string sSize = "";
            const string unsigned = "UNSIGNED";
            if (size > 0) sSize = $"({size})";
            if (type == typeof(int)) return $"INT{sSize}";
            if (type == typeof(uint)) return $"INT{sSize} {unsigned}";
            if (type == typeof(short)) return $"TINYINT{sSize}";
            if (type == typeof(ushort)) return $"TINYINT{sSize} {unsigned}";
            if (type == typeof(string)) return $"VARCHAR{sSize}";
            if (type == typeof(bool)) return "BOOLEAN";
            if (type == typeof(float)) return $"FLOAT{sSize}";
            if (type == typeof(double)) return $"Double{sSize}";
            if (type == typeof(DateTime) || type == typeof(MySqlDateTime)) return "DATETIME";
            if (type == typeof(MySqlDecimal) || type == typeof(decimal)) return "DECIMAL";
            throw new Exception();
        }
        public static string FormatTypeProperties(bool notNull, bool autoIncrement, bool unique) {
            string result = "";
            if (notNull) result += "NOT NULL ";
            if (autoIncrement) result += "AUTO_INCREMENT ";
            if (unique) result += "UNIQUE ";
            return result;
        }
    }

    public static class SqlSupportedTypes {
        public static Type Bool = typeof(bool);
        public static Type Decimal = typeof(MySqlDecimal);
        public static Type Float = typeof(float);
        public static Type Int = typeof(int);
        public static Type Short = typeof(short);
        public static Type String = typeof(string);
        public static Type Time = typeof(MySqlDateTime);
        public static Type Uint = typeof(uint);
        public static Type Ushort = typeof(ushort);
    }
}