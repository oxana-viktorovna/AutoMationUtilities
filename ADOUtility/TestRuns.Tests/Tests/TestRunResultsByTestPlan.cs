using ADOCore;
using ADOCore.ApiClients;
using ADOCore.HttpClients;
using ADOCore.Models;
using ADOCore.Models.WiqlQuery;
using ADOCore.Steps;
using ADOCore.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.BuildInTypesExtentions;
using SharedCore.FileUtilities;
using SharedCore.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestRuns.Tests
{
    [TestClass]
    public class TestRunResultsByTestPlan
    {
        private AdoSettings adoSettings;
        private TestPlanApiSteps apiSteps;
        private TestRunTestSettings testSettings;
        private TestMgmtHttpClientService testMgmtService;

        private const int TestPlanId = 199475;
        private Dictionary<TestSuiteType, int> NightlySuits = new()
        {
            { TestSuiteType.UI, 265004 },
            { TestSuiteType.NonParallel,264947},
            { TestSuiteType.Analytics, 279185 },
            { TestSuiteType.CDW, 277588 },
            { TestSuiteType.NewRA, 274457 },
            { TestSuiteType.Axe, 278993 }
        };

        private Dictionary<int, string> MajorSuits = new()
        {
            { 280523, "US Analytics" },
            { 271702, "US Full" },
            { 271703, "UK Full" },
            { 271704, "US NonParallel" },
            { 271705, "UK NonParallel" },
            { 278584, "RMI" }
        };

        private Dictionary<int, string> MinorSuits = new()
        {
            { 280538, "US Analytics" },
            { 266785, "US Full" },
            { 266783, "UK Full" },
            { 266787, "US NonParallel" },
            { 266788, "UK NonParallel" }
        };

        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);
            apiSteps = new TestPlanApiSteps(adoSettings);
            testMgmtService = new TestMgmtHttpClientService(adoSettings);
            var testSettingsReader = new SettingsReader("TestRunTestConfig.json");
            testSettings = new TestRunTestSettings(testSettingsReader);
        }

        [TestMethod]
        public void UpdateNightlySuitesQueryStrings()
        {
            var notPassedIds = new List<string>();

            foreach (var suite in NightlySuits)
            {
                if (suite.Key == TestSuiteType.Axe)
                {
                    notPassedIds = testMgmtService.GetTestResult(TestPlanId, suite.Value)
                        .Where(r => r.ErrorMessage != null && !r.ErrorMessage.Contains("Accessibility violations"))
                        .Select(r => r.TestCase.Id).ToList();
                }
                else
                {
                    notPassedIds = testMgmtService.GetTestPoints(TestPlanId, suite.Value)
                        .Where(testPoint => !testPoint.Outcome.Equals("passed", StringComparison.InvariantCultureIgnoreCase))
                        .Select(p => p.TestCase.Id).ToList();
                }
                var queryBuilder = GetNightlySuiteQueryBuilder(suite.Key);
                var newQuery = queryBuilder
                    .AddSourceCondition(WiqlConsnt.Conjunction.And, "[System.Id]", WiqlConsnt.Operator.In, $"({string.Join(",", notPassedIds)})")
                    .Build();

                testMgmtService.UpdateSuiteQueryString(TestPlanId, suite.Value, newQuery);
            }
        }

        private WiqlDirectLinksQueryBuilder GetNightlySuiteQueryBuilder(TestSuiteType suite)
        {
            var queryBuilder = new WiqlDirectLinksQueryBuilder(WiqlConsnt.DirectLinkMode.DoesNotContain)
                .AddAttributesToGet("[System.Id]")
                .AddAttributesToGet(WorkItemFields.GetAdoName("Type"))
                .AddAttributesToGet(WorkItemFields.GetAdoName("Title"))
                .AddAttributesToGet(WorkItemFields.GetAdoName("Priority"))
                .AddAttributesToGet(WorkItemFields.GetAdoName("AssignedTo"))
                .AddAttributesToGet(WorkItemFields.GetAdoName("AreaPath"))

                .AddSourceCondition("[Source].[System.TeamProject] = @project")
                .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Test Case")
                .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomationStatus"), WiqlConsnt.Operator.Equal, "Automated")
                .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.NotEqual, "Closed")
                .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomatedTestStorage"), WiqlConsnt.Operator.Contains, "Tracker.Testing.Automation")
                .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, "[System.Tags]", WiqlConsnt.Operator.Contains, "RMI")

                .AddTargetCondition("[Target].[System.TeamProject] = @project")
                .AddTargetCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Bug")
                .AddTargetCondition(WiqlConsnt.Conjunction.And, "[Custom.HighestAffectedEnv]", WiqlConsnt.Operator.NotEqual, "6 - Feature")
                .AddTargetCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.In, "('New', 'Approved', 'Commited', 'In Development', 'Code Review', 'Ready To Test', 'In Testing', 'Ready For Merge')");

            switch (suite)
            {
                case TestSuiteType.UI:
                    queryBuilder
                        .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("AutomatedTestName"), WiqlConsnt.Operator.Contains, "NonParallel")
                        .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("AutomatedTestName"), WiqlConsnt.Operator.Contains, "Axe_")
                        .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, "[System.Tags]", WiqlConsnt.Operator.Contains, "Analytics");
                    break;
                case TestSuiteType.NonParallel:
                    queryBuilder
                        .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomatedTestName"), WiqlConsnt.Operator.Contains, "NonParallel")
                        .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("AutomatedTestName"), WiqlConsnt.Operator.Contains, "Axe_")
                        .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, "[System.Tags]", WiqlConsnt.Operator.Contains, "Analytics");
                    break;
                case TestSuiteType.Analytics:
                    queryBuilder
                        .AddSourceCondition(WiqlConsnt.Conjunction.And, "[System.Tags]", WiqlConsnt.Operator.Contains, "Analytics")
                        .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("AutomatedTestName"), WiqlConsnt.Operator.Contains, "Axe_")
                        .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("AutomatedTestName"), WiqlConsnt.Operator.Contains, "Negative");
                    break;
                case TestSuiteType.CDW:
                    queryBuilder
                        .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomatedTestName"), WiqlConsnt.Operator.Contains, "CDW")
                        .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomatedTestName"), WiqlConsnt.Operator.Contains, "Negative");
                    break;
                case TestSuiteType.NewRA:
                    queryBuilder
                        .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomatedTestName"), WiqlConsnt.Operator.Contains, "NewRa")
                        .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomatedTestName"), WiqlConsnt.Operator.Contains, "Negative");
                    break;
                case TestSuiteType.Axe:
                    queryBuilder
                        .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomatedTestName"), WiqlConsnt.Operator.Contains, "Axe_");
                    break;
            }

            return queryBuilder;
        }

        [TestMethod]
        public void GetSuiteFailedTestIds()
        {
            var suiteId = 274484;
            var testsIds = apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suiteId);

            Assert.Inconclusive(string.Join(",", testsIds));
        }

        [TestMethod]
        public void GetLastNightlyRunDuration()
        {
            var ids_durations = new Dictionary<int, double>();
            foreach (var suite in NightlySuits)
            {
                var ids_durations_suite = apiSteps.GetSuitePassedTestsDuration(TestPlanId, suite.Value);
                ids_durations.ConcatenateWith2(ids_durations_suite);
            }

            var fileName = "NightlyRunDuration.csv";
            var filePath = Path.Combine(testSettings.SaveFolder, fileName);
            var csv = new CsvWorker(filePath);
            var previous_results = csv.Read();
            var content = new StringBuilder();
            var currentDate = DateTime.Now.ToString("dd-MM-yy");

            if (previous_results == null)
            {
                content.AppendLine($"Test Id,{currentDate}");
                foreach (var idDurationPair in ids_durations)
                    content.AppendLine($"{idDurationPair.Key},{idDurationPair.Value}");

            }
            else
            {
                content.AppendLine($"{string.Join(',', previous_results[0])},{currentDate}");

                for (int i = 1; i < previous_results.Count; i++)
                {
                    var elementToAdd = ",";
                    var id = Convert.ToInt32(previous_results[i][0]);
                    var idDurationPair = ids_durations.FirstOrDefault(idd => idd.Key == id);
                    if (!idDurationPair.Equals(default(KeyValuePair<int, int>)))
                    {
                        elementToAdd = idDurationPair.Value.ToString();
                        ids_durations.Remove(idDurationPair.Key);
                    }

                    content.AppendLine($"{string.Join(',', previous_results[i])},{elementToAdd}");
                }

                var numberOfPreviousRuns = previous_results[0].Count() - 1;
                var emptyRuns = string.Concat(Enumerable.Repeat(",", numberOfPreviousRuns));
                foreach (var idDurationPair in ids_durations)
                {
                    content.AppendLine($"{idDurationPair.Key},{emptyRuns}{idDurationPair.Value}");
                }
            }

            csv.Write(content);
        }

        [TestMethod]
        public void GetSuiteFailedTestIdsNightly()
        {
            var result = new StringBuilder();

            foreach (var suite in NightlySuits)
            {
                if (suite.Key == TestSuiteType.Axe)
                    result.AppendLine("Axe: !!! To Get non axe failures Ids Run GetAxeTestResultsByBuildId_NotPassed_Csv()");

                var testsIds = apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suite.Value);
                result.AppendLine(suite.Value + ": " + string.Join(",", testsIds));
            }

            Assert.Inconclusive(Environment.NewLine + result.ToString());
        }

        [TestMethod]
        public void GetSuiteFailedTestIdsMajorRelease()
        {
            var result = new StringBuilder();
            foreach (var suite in MajorSuits)
            {
                var testsIds = apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suite.Key);
                result.AppendLine(suite.Value + ": " + string.Join(",", testsIds));
            }

            Assert.Inconclusive(Environment.NewLine + result.ToString());
        }

        [TestMethod]
        public void GetSuiteFailedTestIdsMinorRelease()
        {
            var result = new StringBuilder();
            foreach (var suite in MinorSuits)
            {
                var testsIds = apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suite.Key);
                result.AppendLine(suite.Value + ": " + string.Join(",", testsIds));
            }

            Assert.Inconclusive(Environment.NewLine + result.ToString());
        }

        [TestMethod]
        public void GetSuiteFailedTestResults()
        {
            var runId = 3603820; // Go to pipeline stage. Check id in the end of the log for 'UI Test Run' task. E.g. 'Test run id: 3424019'
            var suiteId = 293755;
            var workItemsApiSteps = new WorkItemApiSteps(adoSettings);
            var fileName = $"Failed_UI_suiteId{suiteId}_runId{runId}";
            var filePath = Path.Combine(testSettings.SaveFolder, fileName + ".csv");
            var csv = new CsvWorker(filePath);

            var suitetestsIds = this.apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suiteId);

            var apiSteps = new TestRunApiSteps(adoSettings);
            var allTestResults = apiSteps.GetTrxAttachmentsByRunId(runId);

            var content = ResultReportConverter.ToCsvContent(allTestResults, workItemsApiSteps);
            csv.Write(content);
        }
    }
}
