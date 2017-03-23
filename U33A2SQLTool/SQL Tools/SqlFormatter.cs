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
        public static string FormatString(string value) {
            return $"\"{value}\"";
        }
        public static string FormatStrings(List<string> values) {
            string result = "";
            for (int i = 0; i < values.Count; i++) {
                result += $"'{values[i]}'";
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
        public static string PadCenter(string value, int finalLength) {
            string paddedValue = value;
            int amountToBePadded = finalLength - value.Length;
            int leftPad = amountToBePadded / 2;
            paddedValue = PadLeft(paddedValue, value.Length + leftPad);
            return PadRight(paddedValue, value.Length + amountToBePadded);
        }
        public static string PadLeft(string value, int finalLength) {
            string paddedValue = value;
            while (paddedValue.Length < finalLength)
                paddedValue = " " + paddedValue;
            return paddedValue;
        }
        public static string PadRight(string value, int finalLength) {
            string paddedValue = value;
            while (paddedValue.Length < finalLength)
                paddedValue += " ";
            return paddedValue;
        }
    }
}