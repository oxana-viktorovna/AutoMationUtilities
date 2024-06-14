using ADOCore;
using ADOCore.ApiClients;
using ADOCore.Models;
using RestSharp;
using System.Net;

namespace TestRuns.Steps
{
    public class TestPlanApiSteps
    {
        public TestPlanApiClient client { get; private set; }

        public TestPlanApiSteps(AdoSettings adoSettings)
        {
            client = new TestPlanApiClient(adoSettings);
        }

        public TestSuiteTests GetSuiteTests(int testPlanId ,int suiteId)
        => client.GetSuiteTests(testPlanId, suiteId);

        public string GetRunIdForSuite(int suiteId)
            => client.GetRunsForSuite(suiteId);

        public IEnumerable<int> GetSuiteFailedTestIds(int testPlanId, int suiteId)
        {
            var failed = GetSuiteFailedTestPoint(testPlanId, suiteId);
            var failedIds = failed.Select(testPoint => testPoint.testCaseReference.id);

            return failedIds;
        }

        public IEnumerable<int> GetSuiteFailedTestRunIds(int testPlanId, int suiteId)
        {
            var failed = GetSuiteFailedTestPoint(testPlanId, suiteId);
            var runIds = failed.Select(testPoint => testPoint.results.lastTestRunId);

            return runIds;
        }

        private IEnumerable<TestPoint> GetSuiteFailedTestPoint(int testPlanId, int suiteId)
        {
            var response = client.GetSuiteTestPoints(testPlanId, suiteId);
            var failed = response.Where(testPoint => testPoint.results.outcome == "failed");

            return failed;
        }
    }
}
