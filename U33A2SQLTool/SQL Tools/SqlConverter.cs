using U33A2SQLTool.Models.Tables;

namespace U33A2SQLTool.SQL_Tools {
    public class SqlConverter {
        private SqlManager _sqlManager;

        public SqlConverter(SqlManager sqlManager) {
            _sqlManager = sqlManager;
        }
        public Address ConvertToSqlType(Address value) {
            Address sqlAddress = new Address();

            return null;
        }
    }
}