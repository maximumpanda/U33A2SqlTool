using System;
using System.Collections.Generic;
using System.Linq;

namespace U33A2SQLTool.SQL.BaseTypes {
    public class SqlObject : ISqlObject {
        public List<SqlType> Fields { get; set; } = new List<SqlType>();
        public SqlType Id => Fields[0];

        public SqlType this[string id] {
            get { return Fields.FirstOrDefault(property => property.Name == id); }
            set {
                for (int i = 0; i < Fields.Count; i++) {
                    if (Fields[i].Name != id) continue;
                    Fields[i].Parent = this;
                    Fields[i] = value;
                    return;
                }
                Fields.Add(value);
            }
        }
        public SqlType this[int i] {
            get { return Fields[i]; }
            set { Fields[i] = value; }
        }
        public string Name { get; set; }
        public ISqlCollection Parent { get; set; }

        public SqlObject(string name) {
            Name = name;
        }
        public SqlObject(Type type, bool noId = false) {
            Name = type.Name;
            if (noId) return;
            Type idType = typeof(SqlType<>).MakeGenericType(type);
            SqlType id = (SqlType) Activator.CreateInstance(idType);
            id.Name = "Id";
            id.AutoIncrement = true;
            Add(id);
        }

        public SqlObject(SqlObject obj) {
            Fields = obj.Fields;
            Name = obj.Name;
            Parent = obj.Parent;
        }

        public void Add(SqlType obj) {
            obj.Parent = this;
            Fields.Add(obj);
        }
        public string As(string alias) {
            return $"{Name} AS {alias}";
        }
        public string BuldDefinition() {
            string result = "";
            string references = "\nPRIMARY KEY (Id),";
            foreach (ISqlType property in Fields) {
                result += property.FormatDefinition();
                references += property.FormatReference();
            }
            references = references.Remove(references.Length - 1);
            result += references;
            return result;
        }
        public string FormatNames(bool includingId = false) {
            List<string> names = new List<string>();
            foreach (ISqlType type in Fields)
                if (type.Name == "Id") {
                    if (includingId) names.Add(type.Name);
                }
                else {
                    names.Add(type.Name);
                }
            return string.Join(", ", names);
        }
        public string FormatValues(bool includeId = false) {
            List<string> values = new List<string>();
            foreach (ISqlType type in Fields)
                if (type.Name == "Id") {
                    if (includeId) values.Add(type.FormatValue());
                }
                else {
                    values.Add(type.FormatValue());
                }
            return string.Join(", ", values);
        }
        public string SelectId(string field, string value) {
            return $"SELECT Id FROM {Name} WHERE {field} = {value}";
        }
        public override string ToString() {
            return Name;
        }
    }

    public interface ISqlObject {
        List<SqlType> Fields { get; set; }
        SqlType this[string id] { get; set; }
        SqlType this[int i] { get; set; }
        string Name { get; set; }
        ISqlCollection Parent { get; set; }
        void Add(SqlType obj);
        string As(string alias);
        string BuldDefinition();

        string FormatNames(bool includingId);
        string FormatValues(bool includeId);
    }
}