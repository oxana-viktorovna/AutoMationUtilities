using System.Collections.Generic;
using System.Linq;

namespace ADOCore.Models
{
    public static class TestRunExtension
    {
        public static List<TestRunUnitTestResult> GetFailedResults(this IEnumerable<TestRun> testRuns)
        {
            var allResults = testRuns.GetAllResults();
            return allResults.GetFailedResults();
        }

        public static List<TestRunUnitTestResult> GetPassedResults(this IEnumerable<TestRun> testRuns)
        {
            var allResults = testRuns.GetAllResults();
            return allResults.GetPassedResults();
        }

        public static List<OutcomeResult> GetAllTestOutcomes(this IEnumerable<TestRun> testRuns)
        {
            var allResults = testRuns.GetAllResults();
            var testResults = allResults.Select(r => new OutcomeResult(r.testName, r.outcome)).ToList();

            return testResults;
        }

        private static List<TestRunUnitTestResult> GetAllResults(this IEnumerable<TestRun> testRuns)
            => testRuns.SelectMany(run => run.Results).ToList();

        private static List<TestRunUnitTestResult> GetFailedResults(this IEnumerable<TestRunUnitTestResult> allTestResults)
        {
            var grouped = allTestResults
                .GroupBy(result => result.testName);
            var failedGroups = grouped
                .Where(group => !group.Any(gresult => gresult.outcome == "Passed"));
            var failed = failedGroups
                .Select(group => group.Last());

            return failed.ToList();
        }

        private static List<TestRunUnitTestResult> GetPassedResults(this IEnumerable<TestRunUnitTestResult> allTestResults)
        {
            var passed = allTestResults.Where(x => x.outcome == "Passed");

            return passed.ToList();
        }
    }
}
