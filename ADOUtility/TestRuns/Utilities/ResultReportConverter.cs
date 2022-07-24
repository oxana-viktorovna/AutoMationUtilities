using ADOCore.Models;
using SharedCore.StringUtilities;
using System.Collections.Generic;
using System.Linq;
using TestRuns.Models;

namespace TestRuns.Utilities
{
    public static class ResultReportConverter
    {
        public static List<ResultReport> Convert(List<string[]> datas)
        {
            datas.RemoveAt(0);

            return datas.Select(data => Convert(data)).ToList();
        }

        public static ResultReport Convert(string[] data)
            => new ResultReport(
                ConvertToInt(data[0]), 
                data[1], 
                data[2], 
                data[3].TrimStart('"').TrimEnd('"'), 
                data[4]);

        public static List<ResultReport> Convert( List<TestRunUnitTestResult> results)
        {
            var resultReports = new List<ResultReport>();
            foreach (var result in results)
            {
                var testName = result.testName.GetTestMethodName();
                var resultReport = new ResultReport(0, result.testName.GetTestCaseNumber(), testName, result.Output.ErrorInfo.Message.Trim().Replace(',', '-').Replace("\r\n", ". "), "");

                resultReports.Add(resultReport);
            }

            return resultReports;
        }

        private static int ConvertToInt(string data)
            => string.IsNullOrEmpty(data) 
            ? 0 
            : System.Convert.ToInt32(data);
    }
}
