using ADOCore.Models;
using SharedCore.StringUtilities;
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
        {
            var error = data.Count() == 5 
                ? data[4].TrimStart('"').TrimEnd('"')
                : String.Empty;
            return new ResultReport(
                data[1],
                data[2],
                data[3],
                error
                );
        }

        public static List<ResultReport> Convert(List<TestRunUnitTestResult> results)
        {
            var resultReports = new List<ResultReport>();

            for (int i = 0; i < results.Count; i++)
            {
                var testName = results[i].testName.GetTestMethodName();
                var error = results[i].Output.ErrorInfo == null ? "Passed" : results[i].Output.ErrorInfo.Message.Trim().Replace(',', '-').Replace("\r\n", ". ");
                var resultReport = new ResultReport(
                    i + 1, 
                    "", 
                    results[i].testName.GetTestCaseNumber(), 
                    testName,
                    error,
                    results[i].RunName,
                    results[i].Env);

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
