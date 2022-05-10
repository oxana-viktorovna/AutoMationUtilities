using System.Collections.Generic;
using System.Linq;

namespace ADOCore.Models
{
    public static class TestSuitesTestSuiteExteansion
    {
        public static IEnumerable<(string apiName, string request, string test)> GetFailedTests(this testsuitesTestsuite tsuite, string apiName)
            => tsuite.testcase
            .Where(tc => tc.failure != null)
            .Select(tc => (apiName, tsuite.name, tc.name));
    }
}
