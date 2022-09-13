using System.Collections.Generic;
using System.Linq;

namespace ADOCore.Models
{
    public static class TestRunUnitTestResultExtension
    {
        public static List<TestRunUnitTestResult> GetPassedResults(this IEnumerable<TestRunUnitTestResult> allTestResults)
            => allTestResults.Where(x => x.outcome == "Passed").ToList();

        public static List<TestRunUnitTestResult> GetFailedResults(this IEnumerable<TestRunUnitTestResult> allTestResults)
        {
            var groups = allTestResults
                .GroupBy(r => r.testName);
            var failedGroups = groups.Where(gr => !gr.Where(r => r.outcome == "Passed").Any());
            var results = failedGroups.Select(gr => gr.Last()).ToList();

            return results;
        }
    }
}
