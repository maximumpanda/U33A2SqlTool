using System.Collections.Generic;
using System.Linq;

namespace U33A2SQLTool.SQL.BaseTypes {
    public class SqlDatabaseModel {
        public SqlObject this[string name] {
            get { return Tables.FirstOrDefault(table => table.Name == name); }
            set { Tables.Add(value); }
        }
        public string Name { get; set; }
        public List<SqlObject> Tables { get; set; } = new List<SqlObject>();
        public SqlDatabaseModel(string name) {
            Name = name;
        }
    }
}