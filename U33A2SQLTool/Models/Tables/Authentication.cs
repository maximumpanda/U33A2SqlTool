using U33A2SQLTool.SQL.BaseTypes;

namespace U33A2SQLTool.Models.Tables {
    public class Authentication : SqlObject {
        public SqlType Email => this["Email"];
        public SqlType PasswordHash => this["PasswordHash"];
        public SqlType Salt => this["Salt"];
        public Authentication() : base(typeof(uint)) {
            Name = "Authentication";
            Add(new SqlType<string>("Email", 150));
            Add(new SqlType<string>("PasswordHash", 50));
            Add(new SqlType<string>("Salt", 10));
        }
    }
}