using System.Collections.Generic;
using U33A2SQLTool.SQL_Tools;

namespace U33A2SQLTool.SQL.BaseTypes {
    public class SqlCollection : ISqlCollection {
        public int Count => Rows.Count;
        public SqlObject this[int id] {
            get { return Rows[id]; }
            set { Rows[id] = value; }
        }
        public SqlObject Model { get; set; }
        public string Name { get; }
        public List<SqlObject> Rows { get; set; } = new List<SqlObject>();
        public SqlCollection() {
            Name = "Results";
        }
        public SqlCollection(string name) {
            Name = name;
        }
        public SqlCollection(SqlObject model) {
            Model = model;
            Name = SqlHelper.Pluralize(model.Name);
        }
        public void Add(SqlObject obj) {
            obj.Parent = this;
            Rows.Add(obj);
        }
    }

    public interface ISqlCollection {
        int Count { get; }
        SqlObject this[int id] { get; set; }
        string Name { get; }
        List<SqlObject> Rows { get; set; }
        void Add(SqlObject obj);
    }
}