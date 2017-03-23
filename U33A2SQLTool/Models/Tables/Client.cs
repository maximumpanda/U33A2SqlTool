using U33A2SQLTool.SQL.BaseTypes;

namespace U33A2SQLTool.Models.Tables {
    public class Client : SqlObject {
        public Client() : base(typeof(uint)) {
            Name = "Client";
            Add(new SqlType<string>("FirstName", 50));
            Add(new SqlType<string>("LastName", 100));
            Add(new SqlType<uint>("Address", 0, true));
            Add(new SqlType<bool>("IsBusinessAccount"));
            Add(new SqlType<string>("Telephone", 15));
        }
    }
}