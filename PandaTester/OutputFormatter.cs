namespace PandaTester {
    internal static class OutputFormatter {
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