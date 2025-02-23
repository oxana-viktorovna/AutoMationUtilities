using System.Collections.Generic;
using System.Linq;

namespace ADOCore.Models
{
    public static class TestRunUnitTestResultExtension
    {
        public static List<TestRunUnitTestResult> GetPassedResults(this IEnumerable<TestRunUnitTestResult> allTestResults)
            => allTestResults.Where(x => x.outcome == "Passed").ToList();

        /// <summary>
        /// Getting Not passed results of UI tests. Axe is not included
        /// </summary>
        /// <param name="allTestResults"></param>
        /// <returns></returns>
        public static List<TestRunUnitTestResult> GetNotPassedResults(this IEnumerable<TestRunUnitTestResult> allTestResults)
        {
            var groups = allTestResults
                .Where(r => !r.testName.ToLower().Contains("axe"))
                .GroupBy(r => r.testName);
            var failedGroups = groups.Where(gr => !gr.Where(r => r.outcome == "Passed").Any());
            var results = failedGroups.Select(gr => gr.Last()).ToList();

            return results;
        }

        /// <summary>
        /// Getting Not passed results of Axe tests.
        /// </summary>
        /// <param name="allTestResults"></param>
        /// <returns></returns>
        public static List<TestRunUnitTestResult> GetAxeNotPassedResults(this IEnumerable<TestRunUnitTestResult> allTestResults)
        {
            var groups = allTestResults
                .Where(r => r.testName.ToLower().Contains("axe"))
                .GroupBy(r => r.testName);

            var failedGroups = groups.Where(gr => !gr.Where(r => r.outcome == "Passed").Any());

            var results = failedGroups.Select(gr =>
            {
                var lastResult = gr.Last();
                if (lastResult.Output?.ErrorInfo?.Message != null &&
                    !lastResult.Output.ErrorInfo.Message.Contains("Accessibility violations"))
                {
                    lastResult.outcome = "NON-A11y FAILURE";
                }
                return lastResult;
            }).ToList();

            return results;
        }

        public static List<TestRunUnitTestResult> GetPassedOnReRunResults(this IEnumerable<TestRunUnitTestResult> allTestResults)
        {
            var groups = allTestResults
                .GroupBy(r => r.testName);
            var passedOnReRunGroups = groups.Where(gr => gr.Where(r => r.outcome == "Passed").Any() && gr.Where(r => r.outcome == "Failed").Any());
            var results = passedOnReRunGroups.Select(gr => gr.Where(r => r.outcome == "Failed").First()).ToList();

            return results;
        }

        public static List<TestRunUnitTestResult> ExcludeBlocked(this IEnumerable<TestRunUnitTestResult> allTestResults)
            => allTestResults.Where(r => !r.RunName.Contains("Block")).ToList();
    }
}
