using ADOCore.ApiClients;
using ADOCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ADOCore.Steps
{
    public class TestPlanApiSteps
    {
        public TestPlanApiClient client { get; private set; }

        public TestPlanApiSteps(AdoSettings adoSettings)
        {
            client = new TestPlanApiClient(adoSettings);
        }

        public IEnumerable<int> GetSuiteNotPassedTestIds(int testPlanId, int suiteId)
        {
            var testPoints = GetSuiteNotPassedTestPoints(testPlanId, suiteId);
            var failedIds = testPoints.Select(testPoint => testPoint.testCaseReference.id);

            return failedIds;
        }

        public Dictionary<int, double> GetSuitePassedTestsDuration(int testPlanId, int suiteId)
        {
            var testPoints = GetSuitePassedTestPoints(testPlanId, suiteId);
            var result = new Dictionary<int, double>();

            foreach (var testPoint in testPoints)
            {
                var id = testPoint.testCaseReference.id;
                var durationSec = Math.Round(testPoint.results.lastResultDetails.duration*0.001,2);

                if (result.ContainsKey(id))
                {
                    if (durationSec != 0)
                        result[id] = durationSec;
                }
                else
                    result[id] = durationSec;
            }

            return result;
        }

        private IEnumerable<int> GetSuitePassedTestIds(int testPlanId, int suiteId)
        {
            var response = client.GetSuiteTestPoints(testPlanId, suiteId);
            var passed = response.Where(testPoint => testPoint.results.outcome == "passed")
                .Select(testPoint => testPoint.testCaseReference.id);

            return passed;
        }
        private IEnumerable<TestPoint> GetSuiteNotPassedTestPoints(int testPlanId, int suiteId)
        {
            var response = client.GetSuiteTestPoints(testPlanId, suiteId);
            var failed = response.Where(testPoint => testPoint.results.outcome != "passed");

            return failed;
        }

        private IEnumerable<TestPoint> GetSuitePassedTestPoints(int testPlanId, int suiteId)
        {
            var response = client.GetSuiteTestPoints(testPlanId, suiteId);
            var failed = response.Where(testPoint => testPoint.results.outcome == "passed");

            return failed;
        }
    }
}
