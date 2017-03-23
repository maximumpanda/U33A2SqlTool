using U33A2SQLTool.SQL.BaseTypes;

namespace U33A2SQLTool.Models.Tables {
    public class Status : SqlObject {
        public Status() : base(typeof(ushort)) {
            Name = "Status";
            Add(new SqlType<string>("Status", 30, false, true));
        }
    }
}