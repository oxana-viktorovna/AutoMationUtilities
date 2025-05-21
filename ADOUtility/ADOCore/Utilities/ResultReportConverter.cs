using ADOCore.Models;
using ADOCore.Steps;
using SharedCore.StringUtilities;
using System;
using System.Collections.Generic;

namespace ADOCore.Utilities
{
    public static class ResultReportConverter
    {
        public static IEnumerable<string> ToCsvContent(List<TestRunUnitTestResult> results, WorkItemApiSteps? workItemApiSteps = null)
        {
            var result = new List<string>();

            for (int i = 0; i < results.Count; i++)
            {
                var testName = results[i].testName.GetTestMethodName();
                var testId = results[i].testName.GetTestCaseNumber();
                var error = GetErrorMessage(results[i]);

                string areaPath = string.Empty;
                if (workItemApiSteps != null)
                {
                    var converted = Int32.TryParse(testId, out int intTestId);
                    if(converted)
                        areaPath = workItemApiSteps.GetWorkItem(intTestId).fields.AreaPath;
                }

                var line = new string[] {
                    (i + 1).ToString(),
                    results[i].outcome,
                    testId,
                    testName,
                    error,
                    areaPath
                };

                result.Add(string.Join(',', line));
            }

            return result;
        }

        private static string GetErrorMessage(TestRunUnitTestResult result)
        {
            var error = result.Output.ErrorInfo == null
                        ? "Passed"
                        : result.Output.ErrorInfo.Message.Trim().Replace(',', '-').Replace(Environment.NewLine, " ");

            if(error.Length > 32760)
                error = error.Substring(0, 32760);

            return error;
        }
    }
}
