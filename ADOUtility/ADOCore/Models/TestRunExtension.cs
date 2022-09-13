using System.Collections.Generic;
using System.Linq;

namespace ADOCore.Models
{
    public static class TestRunExtension
    {
        public static IEnumerable<TestRunUnitTestResult> GetAllRunResults(this IEnumerable<TestRun> testRuns)
            => testRuns.SelectMany(run => run.Results);
    }
}
