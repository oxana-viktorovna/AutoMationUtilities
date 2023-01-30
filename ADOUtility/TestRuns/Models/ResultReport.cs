using System.Text;

namespace TestRuns.Models
{
    public class ResultReport
    {
        public ResultReport(
            int num,
            string reason,
            string testcase, 
            string testMethod, 
            string error,
            string runName="",
            string buildName = ""
            )
        {
            Number = num;
            TestCaseNumber = testcase;
            TestMethodName = testMethod;
            Error = "\"" + error.Replace("\r\n", "").Replace("\n", "").Replace(",", "") + "\"";
            Reason = reason;
            RunName = runName;
            BuildName = buildName;
        }

        public ResultReport(
            string reason,
            string testcase,
            string testMethod,
            string error,
            string runName="",
            string buildName = ""
            )
        {
            Number = 0;
            TestCaseNumber = testcase;
            TestMethodName = testMethod;
            Error = "\"" + error.Replace("\r\n", "").Replace("\n", "").Replace(",", "") + "\"";
            Reason = reason;
            RunName = runName;
            BuildName = buildName;
        }

        public int Number { get; set; }
        public string TestCaseNumber { get; set; }
        public string TestMethodName { get; set; }
        public string Error { get; set; }
        public string Reason { get; set; }
        public string RunName { get; set; }
        public string BuildName { get; set; }
    }

    public static class ResultReportExtension
    {
        public static string ToCsvLine(this ResultReport result, string splitter)
        => result.Number + splitter 
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
