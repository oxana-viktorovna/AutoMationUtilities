using ADOCore;
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

        private Dictionary<TestSuiteType, int> MajorSuitsUS = new()
        {
            { TestSuiteType.Analytics, 280523 },
            { TestSuiteType.UI, 271702 },
            { TestSuiteType.NonParallel, 271704 },
            { TestSuiteType.RMI, 278584 }
        };

        private Dictionary<TestSuiteType, int> MajorSuitsUK = new()
        {
            { TestSuiteType.UI, 271703 },
            { TestSuiteType.NonParallel, 271705 }
        };

        private Dictionary<TestSuiteType, int> MinorSuitsUS = new()
        {
            { TestSuiteType.Analytics, 280538 },
            { TestSuiteType.UI, 266785 },
            { TestSuiteType.NonParallel, 266787 }
        };

        private Dictionary<TestSuiteType, int> MinorSuitsUK = new()
        {
            { TestSuiteType.UI, 266783 },
            { TestSuiteType.NonParallel, 266788 }
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
        public void RevertNightlySuitesQueryStrings()
        {
            var notPassedIds = new List<string>();

            foreach (var suite in NightlySuits)
            {
                var query = GetNightlySuiteQueryBuilder(suite.Key).Build();

                testMgmtService.UpdateSuiteQueryString(TestPlanId, suite.Value, query);
            }
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
                if (!notPassedIds.Any())
                    notPassedIds.Add("0");
                var queryBuilder = GetNightlySuiteQueryBuilder(suite.Key);
                var newQuery = queryBuilder
                    .AddSourceCondition(WiqlConsnt.Conjunction.And, "[System.Id]", WiqlConsnt.Operator.In, $"({string.Join(",", notPassedIds)})")
                    .Build();

                testMgmtService.UpdateSuiteQueryString(TestPlanId, suite.Value, newQuery);
            }
        }

        [TestMethod]
        public void UpdateToAutomateSuitesQueryStrings()
        {
            var wiqlSteps = new WiqlQueryApiSteps(adoSettings);

            #region new Feature

            var newFeatures = new List<int>
            {
                168820,172360,189893,256416,256439,258296,259741,260120,261436,262320,266672,271591,
                273741,273602,274744,274941,278275,282026,274759,282026,282760,278290,285494,
                286444,287226,289149,289386,289550,289749,289871,290251,292928,293450
            };

            var newFeaturePBIsQuery = new WiqlDirectLinksQueryBuilder().AddAttributesToGet("[System.Id]")
                .AddTargetCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Feature")
                .AddTargetCondition(WiqlConsnt.Conjunction.And, "[System.Id]", WiqlConsnt.Operator.In, "(" + string.Join(',', newFeatures) + ")")
                .AddSourceCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.In, "('Product Backlog Item','Bug')")
                .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.In, "('Removed', 'New', 'Approved')")
                .Build();
            var newFeaturepbis = wiqlSteps.GetLinkedItems(newFeaturePBIsQuery);
            var newFeaturepbiIds = newFeaturepbis.Select(pbi => pbi.source.id).Distinct();

            var newFeatureTestsQuery = new WiqlDirectLinksQueryBuilder().AddAttributesToGet("[System.Id]")
                .AddTargetCondition(null, "[System.Id]", WiqlConsnt.Operator.In, "(" + string.Join(',', newFeaturepbiIds) + ")")
                .AddSourceCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Test Case")
                .Build();
            var newFeaturetests = wiqlSteps.GetLinkedItems(newFeatureTestsQuery);
            var newFeaturetestIds = newFeaturetests.Select(test => test.source.id).Distinct();
            var newFeatureTestsQueryString = GetToAutomateSuiteQuery(newFeaturetestIds, new List<int> { 0, 1});

            testMgmtService.UpdateSuiteQueryString(TestPlanId, 296683, newFeatureTestsQueryString);

            #endregion new Feature

            #region regession

            var non2025inactiveFeatures = new List<int>
            {
                239598
            };

            var inactiveFeaturesQuery = new WiqlFlatQueryBuilder().AddAttributesToGet("[System.Id]")
                .AddCondition(WorkItemFields.GetAdoName("Type"), "Feature", WiqlConsnt.Operator.Equal, null)
                .AddCondition($"{WiqlConsnt.Conjunction.And} [System.TeamProject] = 'Tracker'")
                .AddCondition(WorkItemFields.GetAdoName("IterationPath"), "Tracker\\2025", WiqlConsnt.Operator.Under, WiqlConsnt.Conjunction.And)
                .AddCondition($"{WiqlConsnt.Conjunction.And}(")
                .AddCondition($"{WorkItemFields.GetAdoName("State")} {WiqlConsnt.Operator.In} ('Removed','In Design','New')")
                .AddCondition("System.Tags", "on hold", WiqlConsnt.Operator.Contains, WiqlConsnt.Conjunction.Or)
                .AddCondition("System.Tags", "On-Hold", WiqlConsnt.Operator.Contains, WiqlConsnt.Conjunction.Or)
                .AddCondition(")")
                .Build();
            var inactiveFeatures = wiqlSteps.GetItems(inactiveFeaturesQuery);
            var inactiveFeaturesIds = inactiveFeatures.Select(f => f.id).Distinct().Concat(non2025inactiveFeatures);
            var correctF = inactiveFeaturesIds.FirstOrDefault(f => f == 256438);

            var inactiveFeaturesPBIsQuery = new WiqlDirectLinksQueryBuilder().AddAttributesToGet("[System.Id]")
                .AddSourceCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.In, "('Product Backlog Item','Bug')")
                .AddTargetCondition(null, "[System.Id]", WiqlConsnt.Operator.In, "(" + string.Join(',', inactiveFeaturesIds) + ")")
                .Build();
            var inactiveFeaturesPbis = wiqlSteps.GetLinkedItems(inactiveFeaturesPBIsQuery);
            var inactiveFeaturesPbiIds = inactiveFeaturesPbis.Select(pbi => pbi.source.id).Distinct();
            var correctP = inactiveFeaturesPbiIds.FirstOrDefault(p => p == 259284);

            var regressionTestsQuery = new WiqlDirectLinksQueryBuilder(WiqlConsnt.DirectLinkMode.DoesNotContain).AddAttributesToGet("[System.Id]")
                .AddSourceCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Test Case")
                .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, "[System.Id]", WiqlConsnt.Operator.In, "(" + string.Join(',', newFeaturetestIds) + ")")
                .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("Priority"), WiqlConsnt.Operator.In, "(0,1)")
                .AddTargetCondition(null, "[System.Id]", WiqlConsnt.Operator.In, "(" + string.Join(',', inactiveFeaturesPbiIds) + ")")
                .Build();
            var regTests = wiqlSteps.GetItems(regressionTestsQuery);
            var regTestIds = regTests.Select(test => test.id).Distinct();
            var correctT = regTestIds.FirstOrDefault(t => t == 272067);

            var regQueryP0 = GetToAutomateSuiteQuery(regTestIds, new List<int> { 0 });
            testMgmtService.UpdateSuiteQueryString(TestPlanId, 296687, regQueryP0);

            var regQueryP1 = GetToAutomateSuiteQuery(regTestIds, new List<int> { 1 });
            testMgmtService.UpdateSuiteQueryString(TestPlanId, 296688, regQueryP1);

            #endregion regession
        }

        private string GetToAutomateSuiteQuery(IEnumerable<int> testIds, IEnumerable<int> priorities)
            => new WiqlDirectLinksQueryBuilder(WiqlConsnt.DirectLinkMode.DoesNotContain)
                    .AddAttributesToGet("[System.Id]")
                    .AddAttributesToGet(WorkItemFields.GetAdoName("Type"))
                    .AddAttributesToGet(WorkItemFields.GetAdoName("Title"))
                    .AddAttributesToGet(WorkItemFields.GetAdoName("Priority"))
                    .AddAttributesToGet(WorkItemFields.GetAdoName("AssignedTo"))
                    .AddAttributesToGet(WorkItemFields.GetAdoName("AreaPath"))

                    .AddSourceCondition("[Source].[System.TeamProject] = @project")
                    .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Test Case")
                    .AddSourceCondition(WiqlConsnt.Conjunction.And, "[System.Id]", WiqlConsnt.Operator.In, "(" + string.Join(',', testIds) + ")")
                    .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomationStatus"), WiqlConsnt.Operator.NotEqual, "Automated")
                    .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomationPriority"), WiqlConsnt.Operator.NotEqual, 4)
                    .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.NotEqual, "Closed")
                    .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, "System.Tags", WiqlConsnt.Operator.Contains, "UX Regression")
                    .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("Title"), WiqlConsnt.Operator.Contains, "API")
                    .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("Priority"), WiqlConsnt.Operator.In, "(" + string.Join(',', priorities) + ")")

                    .AddTargetCondition("[Target].[System.TeamProject] = @project")
                    .AddTargetCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Product Backlog Item")
                    .AddTargetCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("Title"), WiqlConsnt.Operator.Contains, "Auto")
                    .Build();

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
                .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("AutomatedTestType"), WiqlConsnt.Operator.Contains, "feature")
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
                        .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomatedTestName"), WiqlConsnt.Operator.Contains, "Axe_")
                        .AddTargetCondition(WiqlConsnt.Conjunction.AndNot, "[System.Tags]", WiqlConsnt.Operator.Contains, "AXE");
                    break;
            }

            return queryBuilder;
        }

        [TestMethod]
        public void GetSuiteFailedTestIds()
        {
            var suiteId = 278993;
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
            foreach (var suite in MajorSuitsUS)
            {
                var testsIds = apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suite.Value);
                result.AppendLine(suite.Value + ": " + string.Join(",", testsIds));
            }
            foreach (var suite in MajorSuitsUK)
            {
                var testsIds = apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suite.Value);
                result.AppendLine(suite.Value + ": " + string.Join(",", testsIds));
            }

            Assert.Inconclusive(Environment.NewLine + result.ToString());
        }

        [TestMethod]
        public void GetSuiteFailedTestIdsMinorRelease()
        {
            var result = new StringBuilder();
            foreach (var suite in MinorSuitsUS)
            {
                var testsIds = apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suite.Value);
                result.AppendLine(suite.Value + ": " + string.Join(",", testsIds));
            }
            foreach (var suite in MinorSuitsUK)
            {
                var testsIds = apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suite.Value);
                result.AppendLine(suite.Value + ": " + string.Join(",", testsIds));
            }

            Assert.Inconclusive(Environment.NewLine + result.ToString());
        }

        [TestMethod]
        public void GetSuiteFailedTestResults()
        {
            var runId = 296857; // Go to pipeline stage. Check id in the end of the log for 'UI Test Run' task. E.g. 'Test run id: 3424019'
            var suiteId = 199475;
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
