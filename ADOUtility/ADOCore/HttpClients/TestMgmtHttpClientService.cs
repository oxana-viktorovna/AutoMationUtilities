using Microsoft.TeamFoundation.SourceControl.WebApi;
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
            repositoryId = "Tracker";

            var connection = new VssConnection(new Uri(adoSettings.BaseOrgUrl), new VssBasicCredential(string.Empty, adoSettings.Password));
            client = connection.GetClient<TestManagementHttpClient>();
            gitClient = connection.GetClient<GitHttpClient>();
        }

        TestManagementHttpClient client;
        GitHttpClient gitClient;
        string projectName;
        string repositoryId;

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

        public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> GetTestPoints(int testPlanId, int testSuiteId)
            => client.GetPointsAsync(projectName, testPlanId, testSuiteId).Result;

        public IEnumerable<GitPullRequest> GetPRs(DateTime startDate, DateTime endDate)
        {
            var repositoryId = "Tracker";

            var searchCriteria = new GitPullRequestSearchCriteria
            {
                Status = PullRequestStatus.Completed,
                MinTime = startDate,
                MaxTime = endDate
            };

            return gitClient.GetPullRequestsAsync(projectName, repositoryId, searchCriteria)
                .Result;
        }

        public IEnumerable<IdentityRefWithVote> GetPRReviewers(IEnumerable<GitPullRequest> pullRequests)
            => pullRequests.SelectMany(pr => gitClient.GetPullRequestReviewersAsync(projectName, repositoryId, pr.PullRequestId).Result);
    }
}
