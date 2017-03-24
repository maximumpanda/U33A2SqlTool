using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace U33A2SQLTool.SQL_Tools {
    public static class SqlHelper {
        private static readonly PluralizationService Pluralizer =
            PluralizationService.CreateService(CultureInfo.CurrentCulture);
        private static readonly Random Rand = new Random();

        public static string Pluralize(string value) {
            return Pluralizer.Pluralize(value);
        }
        public static bool RandomBool() {
            return Convert.ToBoolean(Rand.Next(2));
        }
        public static float RandomFloat() {
            return (float) Rand.NextDouble();
        }
        public static int RandomInt(int upper) {
            return Rand.Next(1, upper + 1);
        }
    }
}