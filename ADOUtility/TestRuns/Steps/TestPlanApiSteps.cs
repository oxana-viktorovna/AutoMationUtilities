﻿using ADOCore;
using ADOCore.ApiClients;
using ADOCore.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Text;

namespace TestRuns.Steps
{
    public class TestPlanApiSteps
    {
        public TestPlanApiClient client { get; private set; }

        public TestPlanApiSteps(AdoSettings adoSettings)
        {
            client = new TestPlanApiClient(adoSettings);
        }

        public TestSuiteTests GetSuiteTests(int testPlanId, int suiteId)
        => client.GetSuiteTests(testPlanId, suiteId);

        public string GetRunIdForSuite(int suiteId)
            => client.GetRunsForSuite(suiteId);

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

        public IEnumerable<int> GetSuiteNotPassedTestRunIds(int testPlanId, int suiteId)
        {
            var failed = GetSuiteNotPassedTestPoints(testPlanId, suiteId);
            var runIds = failed.Select(testPoint => testPoint.results.lastTestRunId);

            return runIds;
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
