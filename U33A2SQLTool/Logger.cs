using System.IO;

namespace U33A2SQLTool {
    public static class Logger {
        private static string _outputFile = string.Empty;
        public static void PrintLines(string[] lines, string file) {
            using (StreamWriter writer = new StreamWriter(file)) {
                foreach (string line in lines) writer.WriteLine(line);
            }
        }
        public static void PrintLines(string[] lines) {
            if (_outputFile == string.Empty) return;
            using (StreamWriter writer = new StreamWriter(_outputFile)) {
                foreach (string line in lines) writer.WriteLine(line);
                writer.WriteLine();
            }
        }
        public static void Report(string value) {
            if (_outputFile == string.Empty) return;
            using (StreamWriter writer = new StreamWriter(_outputFile, true)) {
                writer.WriteLine(value + "\n");
            }
        }

        public static void SetOutputFile(string outputFile) {
            _outputFile = outputFile;
        }
    }
}