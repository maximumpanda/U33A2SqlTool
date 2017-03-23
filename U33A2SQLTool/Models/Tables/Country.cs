using U33A2SQLTool.SQL.BaseTypes;

namespace U33A2SQLTool.Models.Tables {
    public class Country : SqlObject {
        public Country() : base(typeof(ushort)) {
            Name = "Country";
            Add(new SqlType<string>("Name", 50));
            Add(new SqlType<ushort>("Region", 0, true));
        }
    }
}