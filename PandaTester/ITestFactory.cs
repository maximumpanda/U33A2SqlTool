using System.Collections.Generic;

namespace PandaTester {
    public interface ITestFactory {
        List<ITest> Tests { get; set; }
    }
}