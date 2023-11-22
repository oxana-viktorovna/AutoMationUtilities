using System.Collections.Generic;
using System.Linq;

namespace ADOCore.Models
{
    public static class TestRunUnitTestResultExtension
    {
        public static List<TestRunUnitTestResult> GetPassedResults(this IEnumerable<TestRunUnitTestResult> allTestResults)
            => allTestResults.Where(x => x.outcome == "Passed").ToList();

        public static List<TestRunUnitTestResult> GetNotPassedResults(this IEnumerable<TestRunUnitTestResult> allTestResults)
        {
            var groups = allTestResults
                .GroupBy(r => r.testName);
            var failedGroups = groups.Where(gr => !gr.Where(r => r.outcome == "Passed").Any());
            var results = failedGroups.Select(gr => gr.Last()).ToList();

            return results;
        }

        public static List<TestRunUnitTestResult> GetNotExecutedResults(this IEnumerable<TestRunUnitTestResult> allTestResults)
        {
            var groups = allTestResults
                   .GroupBy(r => r.testName);
            var failedGroups = groups.Where(gr => gr.Where(r => r.outcome == "NotExecuted").Any());
            var results = failedGroups.Select(gr => gr.Last()).ToList();

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
