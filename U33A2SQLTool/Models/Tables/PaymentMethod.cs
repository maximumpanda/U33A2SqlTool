using U33A2SQLTool.SQL.BaseTypes;

namespace U33A2SQLTool.Models.Tables {
    public class PaymentMethod : SqlObject {
        public PaymentMethod() : base(typeof(ushort)) {
            Name = "PaymentMethod";
            Add(new SqlType<string>("Method", 20, false, true));
        }
    }
}