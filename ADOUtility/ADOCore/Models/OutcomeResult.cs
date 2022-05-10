using SharedCore.StringUtilities;

namespace ADOCore.Models
{
    public class OutcomeResult
    {
        public string TestId { get; set; }
        public string TestName { get; set; }
        public string Outcome { get; set; }

        public OutcomeResult(string name, string outcome)
        {
            TestId = StringParser.GetTestCaseNumber(name);
            TestName = name;
            Outcome = outcome;
        }
    }
}
