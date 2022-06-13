using ADOCore.Models;
using SharedCore.StringUtilities;
using System.Collections.Generic;
using System.Text;
using TestRuns.Models;

namespace TestRuns.Utilities
{
    public static class TestRunUnitTestResultParserExtension
    {
        public static StringBuilder ToCsvFormat(this List<TestRunUnitTestResult> testResults, string splitter)
        {
            var csv = new StringBuilder();
            var headers = new List<string>()
            {
            "N",
            "Test Case N",
            "Test Method Name",
            "Error",
            "Comment"
            };
            csv.AppendLine(string.Join(splitter, headers));

            for (int i = 0; i < testResults.Count; i++)
            {
                var results = new ResultReport(
                i + 1,
                testResults[i].testName.GetTestCaseNumber(),
                testResults[i].testName.GetTestMethodName(),
                testResults[i].Output.ErrorInfo.Message,
                TestRunConstants.DefaultComment
                );

                csv.AppendLine(results.ToCsvLine(splitter));
            }

            return csv;
        }
    }
}
