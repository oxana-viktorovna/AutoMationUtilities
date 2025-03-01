using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ADOCore.HttpClients
{
    public class TestMgmtHttpClientService
    {
        public TestMgmtHttpClientService(AdoSettings adoSettings)
        {
            projectName = adoSettings.Project;

            var connection = new VssConnection(new Uri(adoSettings.BaseOrgUrl), new VssBasicCredential(string.Empty, adoSettings.Password));
            client = connection.GetClient<TestManagementHttpClient>();
        }

        TestManagementHttpClient client;
        string projectName;

        public string GetSuitsQueryString(int testPlanId, int testSuiteId)
            => client.GetTestSuiteByIdAsync(projectName, testPlanId, testSuiteId).Result.QueryString;

        public void UpdateSuiteQueryString(int testPlanId, int testSuiteId, string newQuery)
        {
            SuiteUpdateModel suiteUpdateModel = new SuiteUpdateModel(queryString: newQuery);
            _ = client.UpdateTestSuiteAsync(suiteUpdateModel, projectName, testPlanId, testSuiteId).Result;
        }

        public IEnumerable<TestCaseResult> GetTestResult(int testPlanId, int testSuiteId)
            => GetTestPoints(testPlanId, testSuiteId)
                .Select(testPoint => client.GetTestResultByIdAsync(projectName, Convert.ToInt32(testPoint.LastTestRun.Id), Convert.ToInt32(testPoint.LastResult.Id)).Result);

        public List<TestPoint> GetTestPoints(int testPlanId, int testSuiteId)
            => client.GetPointsAsync(projectName, testPlanId, testSuiteId).Result;
    }
}
