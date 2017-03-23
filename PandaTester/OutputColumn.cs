using System.Collections.Generic;

namespace PandaTester {
    public class OutputColumn {
        public string Header;
        public int MaxWidth;
        public readonly List<string> Values = new List<string>();
        public int Count => Values.Count;

        public OutputColumn() {
        }
        public OutputColumn(string value) {
            Header = value;
            MaxWidth = Header.Length;
        }
        public void Add(string value) {
            if (value.Length > MaxWidth) MaxWidth = value.Length;
            Values.Add(value);
        }
        public string FormatHeader() {
            return OutputFormatter.PadCenter(Header, MaxWidth);
        }
        public string FormatValue(int index) {
            return OutputFormatter.PadLeft(Values[index], MaxWidth);
        }
    }
}