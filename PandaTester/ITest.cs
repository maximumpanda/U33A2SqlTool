using System;
using System.Collections.Generic;

namespace PandaTester {
    public interface ITest {
        List<Action> Conditions { get; set; }
        List<string> ExpectedValues { get; set; }
        string FailedReason { get; set; }
        string Name { get; set; }
        List<OutputColumn> OutputTable { get; set; }
        List<Action> PreTestActions { get; set; }
        void LogResult();
        void Run(bool logResult = true);
    }
}