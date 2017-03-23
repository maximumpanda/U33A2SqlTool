using U33A2SQLTool.SQL.BaseTypes;

namespace U33A2SQLTool.Models.Tables {
    public class Region : SqlObject {
        public Region() : base(typeof(ushort)) {
            Name = "Region";
            Add(new SqlType<string>("Name", 50, false, true));
        }
    }
}