using System;
using System.Collections.Generic;
using System.Linq;
using PandaTester;
using U33A2SQLTool.SQL.BaseTypes;
using U33A2SQLTool.SQL_Tools;

namespace U33A2SQLTool.Testing {
    public class SqlTest : ITest {
        public string[] ExpectedFields;
        public int ExpectedRecords;
        public bool IsQuery = true;
        public string NonQueryCheckStatement;
        public SqlCollection Result;
        public SqlManager SqlManager;
        public string Statement;
        public List<Action> Conditions { get; set; } = new List<Action>();
        public List<string> ExpectedValues { get; set; } = new List<string>();
        public string FailedReason { get; set; } = "";
        public string Name { get; set; }
        public List<OutputColumn> OutputTable { get; set; } = new List<OutputColumn>();
        public List<Action> PreTestActions { get; set; } = new List<Action>();

        public SqlTest(SqlManager sqlManager) {
            SqlManager = sqlManager;
        }
        public void AddCondition(Func<SqlTest, bool> condition, string message) {
            Conditions.Add(() => FailedReason = condition(this) ? message : FailedReason);
        }
        private void AddDefaultConditions() {
            AddCondition(a => a.Result.Count == 0 && a.ExpectedRecords != -1, "Empty select result");
            AddCondition(a => a.Result.Count != a.ExpectedRecords && a.ExpectedRecords != -1,
                "Number of Records Mismatch");
            AddCondition(a => a.OutputTable.Count != a.ExpectedFields.Length && a.ExpectedRecords != -1,
                "Field Count Mismatch");
        }
        private void AddDefaultExpectedValues() {
            AddExpectedValue("Record Count", ExpectedRecords.ToString());
            AddExpectedValue("Fields", string.Join(", ", ExpectedFields));
        }
        public void AddExpectedValue(string name, string value) {
            ExpectedValues.Add($"Expected {name}: {value}");
        }
        public void AddPreTestAction(Action<SqlTest> action) {
            PreTestActions.Add(() => action(this));
        }
        private string FormatStatement() {
            string formattedStatement = "";
            string[] split = Statement.Split(new[] {"\n"}, StringSplitOptions.None);
            foreach (string s in split) formattedStatement += "    " + s + "\n";
            return formattedStatement;
        }
        public void LogResult() {
            string result;
            if (FailedReason != "") result = "Result: Failed: " + FailedReason;
            else result = "Result: Success";
            string statement = "Statement:\n" + FormatStatement();
            List<string> lines = new List<string> {$"SqlTest: {Name}", result, statement};
            lines.AddRange(ExpectedValues.ToList());
            lines.Add(OutputTable.Aggregate("|", (current, t) => current + t.FormatHeader() + "|"));
            if (OutputTable.Count != 0)
                for (int i = 0; i < OutputTable[0].Count; i++)
                    lines.Add(OutputTable.Aggregate("|", (current, t) => current + t.FormatValue(i) + "|"));
            Console.WriteLine($"Test: {Name}: {result.Replace("Result: ", "")}");
            Logger.PrintLines(lines.ToArray(), "./Tests/" + Name + ".txt");
        }
        private void ResultToTable(SqlCollection result) {
            if (result.Rows.Count == 0) return;
            foreach (SqlType field in result.Rows[0].Fields) OutputTable.Add(new OutputColumn(field.Name));
            for (int i = 0; i < result.Count; i++) {
                SqlObject obj = result.Rows[i];
                for (int j = 0; j < obj.Fields.Count; j++)
                    if (obj.Fields[j].Type == typeof(bool) && obj.Fields[j].Name == "IsBusinessAccount")
                        OutputTable[j].Add((bool) obj.Fields[j].Value ? "Business" : "Normal");
                    else OutputTable[j].Add(obj.Fields[j].FormatValue());
            }
        }
        public void Run(bool logResult = true) {
            foreach (Action preTestAction in PreTestActions) preTestAction();
            AddDefaultExpectedValues();
            Result = IsQuery ? SqlManager.RunQuery(Statement) : RunNonQuery();
            if (Result != null && FailedReason == "") {
                ResultToTable(Result);
                AddDefaultConditions();
                foreach (Action condition in Conditions) {
                    condition();
                    if (FailedReason != "") break;
                }
            }
            if (logResult) LogResult();
        }
        private SqlCollection RunNonQuery() {
            if (NonQueryCheckStatement == "") {
                FailedReason = "no Check Statement for Non-Query";
                return null;
            }
            SqlManager.RunNonQuery(Statement);
            return SqlManager.RunQuery(NonQueryCheckStatement);
        }
    }
}