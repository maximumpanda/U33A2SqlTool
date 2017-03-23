using U33A2SQLTool.SQL.BaseTypes;

namespace U33A2SQLTool.Models.Tables {
    public class City : SqlObject {
        public SqlType Country => this["Name"];
        public SqlType FieldName => this["Name"];
        public City() : base(typeof(uint)) {
            Name = "City";
            Add(new SqlType<string>("Name", 50));
            Add(new SqlType<ushort>("Country", 1, true));
        }
    }
}