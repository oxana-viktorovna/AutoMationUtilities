using System.Text;

namespace TestRuns.Models
{
    public class ResultReport
    {
        public ResultReport(
            int num,
            string outcome,
            string reason,
            string testcase, 
            string testMethod, 
            string error
            )
        {
            Number = num;
            TestCaseNumber = testcase;
            TestMethodName = testMethod;
            Error = "\"" + error.Replace("\r\n", "").Replace("\n", "").Replace(",", "") + "\"";
            Reason = reason;
            Outcome = outcome;
        }
        public ResultReport(
            int num,
            string outcome,
            string reason,
            string testcase,
            string testMethod,
            string error,
            string areaPath
            )
        {
            Number = num;
            TestCaseNumber = testcase;
            TestMethodName = testMethod;
            Error = "\"" + error.Replace(",", "") + "\"";
            Reason = reason;
            Outcome = outcome;
            AreaPath = areaPath;
        }
        public int Number { get; set; }
        public string TestCaseNumber { get; set; }
        public string TestMethodName { get; set; }
        public string Error { get; set; }
        public string Reason { get; set; }
        public string Outcome { get; set; }
        public string AreaPath { get; set; }
    }

    public static class ResultReportExtension
    {
        public static string ToCsvLine(this ResultReport result, string splitter)
        => result.Number + splitter
            + result.Outcome
            + result.Reason  + splitter 
            + result.TestCaseNumber + splitter 
            + result.TestMethodName + splitter 
            + result.Error;

        public static StringBuilder ToCsvFormat(this IEnumerable<ResultReport> results, string splitter)
        {
            var strbuilder = new StringBuilder();

            var headers = new List<string>()
            {
            "N",
            "OutCome",
            "Reason",
            "Test Case N",
            "Test Method Name",
            "Error"
            };
            strbuilder.AppendLine(string.Join(splitter, headers));

            foreach (var result in results)
            {
                strbuilder.AppendLine(result.ToCsvLine(splitter));
            }

            return strbuilder;
        }
    }
}
