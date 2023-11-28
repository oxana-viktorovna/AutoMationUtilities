using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ADOCore.Models;
using ADOCore.ApiClients;
using SharedCore.FileUtilities;

namespace ADOCore.ApiClients
{
    public class TestPlanApiClient : CoreAdoApiClient
    {
        public TestPlanApiClient(AdoSettings adoSettings) : base(adoSettings)
        {

        }

        public T GetTestPlanDetails<T>(int planId, int? suitId)
        {
            string apiUrl;
            Type responseType;
            string response;
            if (suitId != 0)
            {
                apiUrl = $"testplan/Plans/{planId}/suites/{suitId}?expand=Children&api-version=7.2-preview.1";
            }
            else
            {
                apiUrl = $"testplan/Plans/{planId}/suites?api-version=7.2-preview.1";
            }
            response = SendAdoRequest(apiUrl, Method.GET).Content;
            return JsonSerializer.Deserialize<T>(response);
        }

        public TestCaseResponse GetTestIds(int planId, int suitId)
        {
            var response = SendAdoRequest($"testplan/Plans/{planId}/Suites/{suitId}/TestCase", Method.GET);
            var testPlanInfo = JsonSerializer.Deserialize<TestCaseResponse>(response.Content);
            return testPlanInfo;
        }
    }
}
