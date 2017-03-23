using U33A2SQLTool.SQL.BaseTypes;

namespace U33A2SQLTool.Models.Tables {
    public class Address : SqlObject //SqlBaseType<uint>
    {
        public SqlType City => this["City"];
        public SqlType PostCode => this["PostCode"];
        public SqlType StreetAddress => this["StreetAddress"];

        public Address() : base(typeof(uint)) {
            Name = "Address";
            Add(new SqlType<string>("StreetAddress", 100));
            Add(new SqlType<uint>("City", 0, true));
            Add(new SqlType<string>("PostCode", 10));
        }
    }
}