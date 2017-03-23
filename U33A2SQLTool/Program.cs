using System;

namespace U33A2SQLTool {
    public class Program {
        public static DataManager Manager;

        private static void Main(string[] args) {
            Logger.SetOutputFile("output.txt");
            string dbConnectionString =
                @"server=127.0.0.1;UID=root;password=TestEnvironmentPassword;database=information_Schema";
            Manager = new DataManager(dbConnectionString, "U33A2", true);

            Console.ReadKey();
        }
    }
}