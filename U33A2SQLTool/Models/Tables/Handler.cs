using U33A2SQLTool.SQL.BaseTypes;

namespace U33A2SQLTool.Models.Tables {
    public class Handler : SqlObject {
        public Handler() : base(typeof(uint)) {
            Name = "Handler";
            Add(new SqlType<string>("Name", 100));
            Add(new SqlType<uint>("Address", 0, true));
        }
    }
}