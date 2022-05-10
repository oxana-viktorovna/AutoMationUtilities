using System.Collections.Generic;
using System.Linq;

namespace ADOCore.Models
{
    public static class TestSuitesExtension
    {
        public static List<(string apiName, string request, string test)> GetFailedTests(this testsuites testSuites)
        => testSuites.testsuite
                .Where(tsuite => tsuite.testcase != null)
                .SelectMany(tsuite => tsuite.GetFailedTests(testSuites.name))
                .ToList();

        public static List<(string apiName, string request, string test)> GetFailedTests(this IEnumerable<testsuites> testSuites)
            => testSuites.SelectMany(xmlReport => GetFailedTests(xmlReport)).ToList();
    }
}
