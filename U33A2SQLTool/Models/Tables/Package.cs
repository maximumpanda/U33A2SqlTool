using MySql.Data.Types;
using U33A2SQLTool.SQL.BaseTypes;

namespace U33A2SQLTool.Models.Tables {
    public class Package : SqlObject {
        public Package() : base(typeof(uint)) {
            Name = "Package";
            Add(new SqlType<uint>("Client", 0, true));
            Add(new SqlType<ushort>("Status", 0, true));
            Add(new SqlType<uint>("Handler", 0, true));
            Add(new SqlType<string>("Recipient", 50));
            Add(new SqlType<uint>("Address", 0, true));
            Add(new SqlType<MySqlDateTime>("PickupTime"));
            Add(new SqlType<MySqlDateTime>("DeliveryTime"));
            Add(new SqlType<float>("Weight"));
        }
    }
}