using System.Collections.Generic;

namespace PandaTester {
    public class Tester {
        public List<ITest> Tests;

        public Tester(ISeeder seeder, ITestFactory testFactory) {
            seeder.Seed();
            Tests = testFactory.Tests;
        }

        public void RunAllTests(bool shouldLogResult = true) {
            foreach (ITest test in Tests) test.Run(shouldLogResult);
        }
        public void RunTest(int id, bool shouldLogResult = true) {
            if (id <= Tests.Count) Tests[id].Run(shouldLogResult);
        }
        public void RunTest(string name, bool shouldLogResult = true) {
            foreach (ITest test in Tests) if (test.Name == name) test.Run(shouldLogResult);
        }
    }
}