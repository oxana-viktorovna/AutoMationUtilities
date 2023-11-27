using ADOCore.ApiClients;
using ADOCore;
using ADOCore.Models;
using System.Text.RegularExpressions;

namespace TestCases.Steps
{
    public class TestCaseApiStepsNew
    {
        public TestCaseApiStepsNew(AdoSettings adoSettings)
        {
            testRunApiClient = new TestCaseApiClient(adoSettings);
        }

        private readonly TestCaseApiClient testRunApiClient;

        public List<(string TestCaseId, string TestCaseName)> GetTestIdNamePairs(int planId, int? suitId)
        {
            List<(string AutomatedTestId, string AutomatedTestName)> testPairs = new List<(string, string)>();
            if (suitId != 0)
            {
                var testPlanDetails = testRunApiClient.GetTestPlanDetails<TestPlanDetailsWithNestedSuits>(planId, suitId);
                var nestedSuiteIds = testPlanDetails?.children?.Select(s => s.id).ToList();
                if (nestedSuiteIds != null && nestedSuiteIds.Any())
                {
                    ProcessTestPlanDetails(testPairs, planId, nestedSuiteIds);
                }
            }
            else
            {
                var testPlanDetails = testRunApiClient.GetTestPlanDetails<TestPlanDetailsWithoutNestedSuits>(planId, suitId);
                if (testPlanDetails != null && testPlanDetails.value != null && testPlanDetails.value.Any())
                {
                    var suiteIds = testPlanDetails.value.Select(s => s.id).ToList();
                    ProcessTestPlanDetails(testPairs, planId, suiteIds);
                }
            }
            return testPairs;
        }

        public List<List<(string Id, string Name)>> DivideIntoBatches(List<(string Id, string Name)> testPairs, int batchCount)
        {
            List<List<(string Id, string Name)>> batches = new List<List<(string Id, string Name)>>();
            for (int i = 0; i < batchCount; i++)
            {
                batches.Add(new List<(string Id, string Name)>());
            }
            var classGroup = testPairs.GroupBy(pair => GetClassName(pair.Name));
            foreach (var group in classGroup)
            {
                int currentBatchIndex = 0;
                foreach (var pair in group)
                {
                    batches[currentBatchIndex].Add((pair.Id, pair.Name));
                    currentBatchIndex = (currentBatchIndex + 1) % batchCount;
                }
            }
            return batches;
        }

        private void ProcessTestPlanDetails(List<(string, string)> testPairs, int planId, List<int> suiteIds)
        {
            if (suiteIds != null && suiteIds.Any())
            {
                int selectedSuiteId = suiteIds[1];
                var testCaseResponse = testRunApiClient.GetTestIds(planId, selectedSuiteId);
                if (testCaseResponse != null && testCaseResponse.value != null && testCaseResponse.value.Any())
                {
                    foreach (var testPlanDetail in testCaseResponse.value)
                    {
                        var testIdToken = testPlanDetail.workItem?.workItemFields
                            ?.FirstOrDefault(field => field.ContainsKey("Microsoft.VSTS.TCM.AutomatedTestId"))
                            ?.GetValueOrDefault("Microsoft.VSTS.TCM.AutomatedTestId")?.ToString();
                        var testNameToken = testPlanDetail.workItem?.workItemFields
                            ?.FirstOrDefault(field => field.ContainsKey("Microsoft.VSTS.TCM.AutomatedTestName"))
                            ?.GetValueOrDefault("Microsoft.VSTS.TCM.AutomatedTestName")?.ToString();
                        testPairs.Add((testIdToken, testNameToken));
                    }
                }
                else
                {
                    Console.WriteLine("The response is empty.");
                }
            }
        }

        private string GetClassName(string testName)
        {
            var match = Regex.Match(testName, @".*Tests.");
            if (match.Success)
            {
                return match.Value.Replace(".", "");
            }
            return "UnknownClass";
        }
    }
}
