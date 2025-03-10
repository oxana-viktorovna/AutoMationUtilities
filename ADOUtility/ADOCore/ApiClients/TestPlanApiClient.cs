﻿using ADOCore.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ADOCore.ApiClients
{
    public class TestPlanApiClient : CoreAdoApiClient
    {
        public TestPlanApiClient(AdoSettings adoSettings) : base(adoSettings)
        {
        }

        public IEnumerable<TestPoint> GetSuiteTestPoints(int testPlanId, int suiteId)
        {
            string continuationToken = null;
            var testPoints = new List<TestPoint>();
            do
            {
                var testPointsBatch = GetSuiteTestPointsBatch(testPlanId, suiteId, continuationToken, out continuationToken);
                if(testPointsBatch != null)
                    testPoints.AddRange(testPointsBatch);
            }
            while (!string.IsNullOrEmpty(continuationToken));

            return testPoints;
        }

        private IEnumerable<TestPoint> GetSuiteTestPointsBatch(int testPlanId, int suiteId, string currentContinuationToken, out string nextContinuationToken)
        {
            nextContinuationToken = null;
            var url = $"testplan/Plans/{testPlanId}/suites/{suiteId}/TestPoint";
            var param = new List<(string, string)>();
            if (!string.IsNullOrEmpty(currentContinuationToken))
                param.Add(("continuationToken", currentContinuationToken));
            
            var response = SendAdoRequest(url, Method.Get, param, version: "7.1-preview.2");
            var content = JsonSerializer.Deserialize<TestPointResponse>(response.Content);
            nextContinuationToken = response.Headers
                                    .Where(header => header.Name.Equals("x-ms-continuationtoken", StringComparison.OrdinalIgnoreCase))
                                    .Select(header => header.Value.ToString())
                                    .FirstOrDefault();

            return content.value;
        }
    }
}
