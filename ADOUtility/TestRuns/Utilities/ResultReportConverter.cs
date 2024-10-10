using ADOCore.Models;
using SharedCore.StringUtilities;
using System.Diagnostics;
using System.Text;
using TestRuns.Models;
using TestRuns.Steps;

namespace TestRuns.Utilities
{
    public static class ResultReportConverter
    {
        public static List<ResultReport> Convert(IEnumerable<string[]> datas)
        {
            datas.ToList().RemoveAt(0);

            return datas.Select(data => Convert(data)).ToList();
        }

        public static ResultReport Convert(string[] data)
        {
            var error = data.Count() == 6 
                ? data[5].TrimStart('"').TrimEnd('"')
                : String.Empty;
            return new ResultReport(
                System.Convert.ToInt32(data[0]),
                data[1],
                data[2],
                data[3],
                data[4],
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
                    results[i].outcome,
                    "", 
                    results[i].testName.GetTestCaseNumber(), 
                    testName,
                    error);

                resultReports.Add(resultReport);
            }

            return resultReports;
        }

        public static List<ResultReport> ConvertWithAreaPath(List<TestRunUnitTestResult> results, WorkItemApiSteps workItemApiSteps)
        {
            var resultReports = new List<ResultReport>();

            for (int i = 0; i < results.Count; i++)
            {
                var testId = results[i].testName.GetTestCaseNumber();
                var areaPath = workItemApiSteps.GetWorkItemNew(System.Convert.ToInt32(testId)).fields.SystemAreaPath;
                var testName = results[i].testName.GetTestMethodName();
                var error = results[i].Output.ErrorInfo == null ? "Passed" : results[i].Output.ErrorInfo.Message.Trim().Replace(',', '-');
                var resultReport = new ResultReport(
                    i + 1,
                    results[i].outcome,
                    "",
                    testId,
                    testName,
                    error,
                    areaPath);

                resultReports.Add(resultReport);
            }

            return resultReports;
        }

        public static List<ResultReport> ConvertAxe(List<TestRunUnitTestResult> results, WorkItemApiSteps workItemApiSteps)
        {
            var resultReports = new List<ResultReport>();

            for (int i = 0; i < results.Count; i++)
            {
                var testId = results[i].testName.GetTestCaseNumber();
                var areaPath = workItemApiSteps.GetWorkItemNew(System.Convert.ToInt32(testId)).fields.SystemAreaPath;
                var testName = results[i].testName.GetTestMethodName();
                var error = results[i].Output.ErrorInfo == null 
                    ? "Passed" 
                    : UpdateAxeError(results[i].Output.ErrorInfo.Message);
                var resultReport = new ResultReport(
                    i + 1,
                    results[i].outcome,
                    "",
                    testId,
                    testName,
                    error,
                    areaPath);

                resultReports.Add(resultReport);
            }

            return resultReports;
        }

        private static string UpdateAxeError(string errorMsg)
        {
            errorMsg = StringParser.RemoveTextFromTo(errorMsg, "Report can be found under ", "But was:  True");
            errorMsg = StringParser.RemoveTextFromTo(errorMsg, "https:", "thomsonreuters.com");
            errorMsg = errorMsg.Replace("Accessibility violations found on ", "");      
            errorMsg = errorMsg.Replace("Multiple failures or warnings in test:", "");
            errorMsg = errorMsg.Substring(1).Trim().Replace(',', '-'); ;

            return errorMsg;
        }
    }
}
