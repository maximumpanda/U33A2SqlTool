using MySql.Data.Types;
using U33A2SQLTool.SQL.BaseTypes;

namespace U33A2SQLTool.Models.Tables {
    public class Invoice : SqlObject {
        public Invoice() : base(typeof(uint)) {
            Name = "Invoice";
            Add(new SqlType<uint>("Package", 0, true));
            Add(new SqlType<MySqlDateTime>("Date"));
            Add(new SqlType<float>("Price", 3));
            Add(new SqlType<bool>("IsPaid"));
            Add(new SqlType<ushort>("PaymentMethod", 0, true, false, false));
        }
    }
}