using System;

namespace U33A2SQLTool.SQL.BaseTypes {
    public interface ISqlType {
        bool AutoIncrement { get; set; }
        bool ForeignKey { get; set; }
        string FullName { get; }
        string Name { get; set; }
        bool NotNull { get; set; }
        SqlObject Parent { get; set; }
        string ReferencesTable { get; set; }
        uint Size { get; set; }
        Type Type { get; set; }
        bool Unique { get; set; }
        object Value { get; set; }

        string As(string alias);

        string FormatDefinition();
        string FormatReference();
        string FormatValue();
    }
}