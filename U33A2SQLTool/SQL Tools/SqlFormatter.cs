using System.Collections.Generic;

namespace U33A2SQLTool.SQL_Tools {
    public static class SqlFormatter {
        public static string FormatReferences(List<string> values) {
            string result = "";
            for (int i = 0; i < values.Count; i++) {
                result += $"{values[i]}";
                if (i + 1 < values.Count) result += ", ";
            }
            return result;
        }
        public static string FormatValues(List<object> values) {
            string result = "";
            for (int i = 0; i < values.Count; i++) {
                if (values[i] is string) result += $"\"{(string) values[i]}\"";
                if (values[i] is ushort) result += $"{(ushort) values[i]}";
                if (values[i] is int) result += $"{(int) values[i]}";
                if (i + 1 < values.Count) result += ", ";
            }
            return result;
        }
    }
}