using System.Text;

namespace TestRuns.Models
{
    public class ResultReport
    {
        public ResultReport(
            int num, 
            string testcase, 
            string testMethod, 
            string error, 
            string comment)
        {
            Number = num;
            TestCaseNumber = testcase;
            TestMethodName = testMethod;
            Error = "\"" + error.Replace("\r\n", "").Replace("\n", "").Replace(",", "") + "\"";
            Comment = comment;
        }

        public int Number { get; set; }
        public string TestCaseNumber { get; set; }
        public string TestMethodName { get; set; }
        public string Error { get; set; }
        public string Comment { get; set; }
    }

    public static class ResultReportExtension
    {
        public static string ToCsvLine(this ResultReport result, string splitter)
        => result.Number+ splitter+result.TestCaseNumber + splitter + result.TestMethodName + splitter + result.Error + splitter + result.Comment;

        public static StringBuilder ToCsvFormat(this IEnumerable<ResultReport> results, string splitter)
        {
            var strbuilder = new StringBuilder();

            var headers = new List<string>()
            {
            "N",
            "Test Case N",
            "Test Method Name",
            "Error",
            "Comment"
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
