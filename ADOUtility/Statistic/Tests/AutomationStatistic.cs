using ADOCore;
using ADOCore.Models.WiqlQuery;
using ADOCore.Steps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using Statistic.Settings;
using Statistic.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resources = SharedCore.ResourcesUtilities.ResourcesUtilities;

namespace Statistic.Tests
{
    [TestClass]
    public class AutomationStatistic
    {
        [TestInitialize]
        public void TestInit()
        {
            adoSettings = new AdoSettings(new SettingsReader("ADOconfig.json"));
            autoStatSettings = new AutoStatisticSettings(new SettingsReader("AutoStatisticSettings.json"));
            defaultAreaPathes = autoStatSettings.DefaultAreaPathes;
            autoStatSteps = new AutomationStatisticSteps(adoSettings, defaultAreaPathes, autoStatSettings.AsOf);
        }

        private AdoSettings adoSettings;
        private AutoStatisticSettings autoStatSettings;
        private AutomationStatisticSteps autoStatSteps;
        private List<string> defaultAreaPathes;

        [TestMethod]
        public void GetNewFeatureAutomationStat()
        {
            var message = new StringBuilder(Environment.NewLine + autoStatSettings.AsOf + Environment.NewLine);
            var wiqlSteps = new WiqlQueryApiSteps(adoSettings);
            var newFeatures = Resources.GetNewFeaturesIds(autoStatSettings.AsOf);

            var newFeaturePBIsQuery = new WiqlDirectLinksQueryBuilder().AddAttributesToGet("[System.Id]")
                .AddTargetCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Feature")
                .AddTargetCondition(WiqlConsnt.Conjunction.And, "[System.Id]", WiqlConsnt.Operator.In, "(" + string.Join(',', newFeatures) + ")")
                .AddTargetCondition(WiqlConsnt.Conjunction.AndNot, "System.Tags", WiqlConsnt.Operator.Contains, "on hold")
                .AddTargetCondition(WiqlConsnt.Conjunction.AndNot, "System.Tags", WiqlConsnt.Operator.Contains, "On-Hold")
                .AddTargetCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("AreaPath"), WiqlConsnt.Operator.Under, "Tracker\\Firefly")
                .AddTargetCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.In, "('In Design', 'New')")
                .AddSourceCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.In, "('Product Backlog Item','Bug')")
                .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.In, "('Removed', 'New', 'Approved')")
                .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, "System.Tags", WiqlConsnt.Operator.Contains, "on hold")
                .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, "System.Tags", WiqlConsnt.Operator.Contains, "On-Hold")
                .Build();
            var newFeaturePbis = wiqlSteps.GetLinkedItems(newFeaturePBIsQuery);
            var newFeaturePbiIds = newFeaturePbis.Select(pbi => pbi.source.id).Distinct();

            var newFeatureTestsQuery = new WiqlDirectLinksQueryBuilder().AddAttributesToGet("[System.Id]")
                .AddTargetCondition(null, "[System.Id]", WiqlConsnt.Operator.In, "(" + string.Join(',', newFeaturePbiIds) + ")")
                .AddSourceCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Test Case")
                .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.In, "('Removed', 'New', 'Approved')")
                .Build();
            var newFeatureTests = wiqlSteps.GetLinkedItems(newFeatureTestsQuery);
            var newFeatureTestIds = newFeatureTests.Select(test => test.source.id).Distinct();
            message.AppendLine($"All New Feature Tests {newFeatureTestIds.Count()}:" + string.Join(",", newFeatureTestIds));

            var automatableQueryBuilder = new WiqlFlatQueryBuilder()
                   .AddAttributesToGet("[System.Id]")
                   .AddCondition(null, "[System.Id]", WiqlConsnt.Operator.In, newFeatureTestIds)
                   .AddCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("AutomationPriority"), WiqlConsnt.Operator.Equal, 4)
                   .AddCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("Priority"), WiqlConsnt.Operator.In,new List<int>() { 0, 1 })
                   .AddCondition(WiqlConsnt.Conjunction.AndNot, "System.Tags", WiqlConsnt.Operator.Contains, "UX Regression");
            var automatable = autoStatSteps.GetTestCount(automatableQueryBuilder);
            message.AppendLine($"Automatable: {automatable}");

            var automatedQueryBuilder = new WiqlFlatQueryBuilder()
                   .AddAttributesToGet("[System.Id]")
                   .AddCondition(null, "[System.Id]", WiqlConsnt.Operator.In, newFeatureTestIds)
                   .AddCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomationStatus"), WiqlConsnt.Operator.Equal, "Automated");
            var automated = autoStatSteps.GetTestCount(automatedQueryBuilder);
            message.AppendLine($"Automated: {automated}");

            Assert.Inconclusive(message.ToString());

        }

        [TestMethod]
        public void GetUIAutoTestID()
        {
            var query = new WiqlFlatQueryBuilder()
                   .AddAttributesToGet("[System.Id]")
                   .AddCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Test Case")
                   .AddCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AreaPath"), WiqlConsnt.Operator.Under, "Tracker")
                   .AddCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.NotEqual, "Closed")
                   .AddCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomationStatus"), WiqlConsnt.Operator.Equal, "Automated")
                   .AddCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("Title"), WiqlConsnt.Operator.Contains, "a11y")
                   .Build();
            var wiqlSteps = new WiqlQueryApiSteps(adoSettings);
            var result = wiqlSteps.GetItems(query);
        }

        [TestMethod]
        public void GetUIAutoTestCoverage()
        {
            var uxTag = "UX Regression";
            var otherPriorities = new List<int> { 2, 3, 4 };
            var line = "priority {0} automated {1} all test cases {2} including {3} UX test cases, {4} not automatable";
            var message = new StringBuilder(Environment.NewLine + autoStatSettings.AsOf + Environment.NewLine);

            var priorities = new List<int> { 0, 1 };


            foreach (var priority in priorities)
            {
                var autoQueryBuilder = GetTestCasesQueryBuilder(priority)
                    .AddCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomationStatus"), WiqlConsnt.Operator.Equal, "Automated");
                var auto = autoStatSteps.GetTestCount(autoQueryBuilder);

                var totalQueryBuilder = GetTestCasesQueryBuilder(priority);
                var total = autoStatSteps.GetTestCount(totalQueryBuilder);

                var uxQueryBuilder = GetTestCasesQueryBuilder(priority)
                    .AddCondition(WiqlConsnt.Conjunction.And, "System.Tags", WiqlConsnt.Operator.Contains, "UX Regression")
                    .AddCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("AutomationStatus"), WiqlConsnt.Operator.Equal, "Automated");
                var ux = autoStatSteps.GetTestCount(uxQueryBuilder);

                var noautoQueryBuilder = GetTestCasesQueryBuilder(priority)
                    .AddCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomationPriority"), WiqlConsnt.Operator.Equal, 4)
                    .AddCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("AutomationStatus"), WiqlConsnt.Operator.Equal, "Automated");
                var nonauto = autoStatSteps.GetTestCount(noautoQueryBuilder);

                message.AppendLine(string.Format(line, priority, auto, total, ux, nonauto));
            }

            int autoP2plus = 0;
            int totalP2plus = 0;
            int uxP2plus = 0;
            int nonautoP2plus = 0;
            priorities = new List<int> { 2, 3, 4 };
            foreach (var priority in priorities)
            {
                var autoQueryBuilder = GetTestCasesQueryBuilder(priority)
                    .AddCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomationStatus"), WiqlConsnt.Operator.Equal, "Automated");
                autoP2plus += autoStatSteps.GetTestCount(autoQueryBuilder);

                var totalQueryBuilder = GetTestCasesQueryBuilder(priority);
                totalP2plus += autoStatSteps.GetTestCount(totalQueryBuilder);

                var uxQueryBuilder = GetTestCasesQueryBuilder(priority)
                    .AddCondition(WiqlConsnt.Conjunction.And, "System.Tags", WiqlConsnt.Operator.Contains, "UX Regression");
                uxP2plus += autoStatSteps.GetTestCount(uxQueryBuilder);

                var noautoQueryBuilder = GetTestCasesQueryBuilder(priority)
                    .AddCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomationPriority"), WiqlConsnt.Operator.Equal, 4);
                nonautoP2plus += autoStatSteps.GetTestCount(noautoQueryBuilder);
            }

            message.AppendLine(string.Format(line, "2+", autoP2plus, totalP2plus, uxP2plus, nonautoP2plus));

            Assert.Inconclusive(message.ToString());
        }

        private WiqlFlatQueryBuilder GetTestCasesQueryBuilder(int? priority)
        {
            var builder = new WiqlFlatQueryBuilder()
                  .AddAttributesToGet("[System.Id]")
                  .AddCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Test Case")
                  .AddCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AreaPath"), WiqlConsnt.Operator.Under, "Tracker")
                  .AddCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.NotEqual, "Closed")
                  .AddCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("Title"), WiqlConsnt.Operator.Contains, "a11y");

            if (priority != null)
                builder.AddSinglePriorityCondition(priority.Value, WiqlConsnt.Operator.Equal);

            return builder;
        }

        [TestMethod]
        public void Get255Scope()
        {
            var tag = "255scope";
            var stistic = new StringBuilder("AreaPath \t total \t auto");
            stistic.AppendLine();
            foreach (var areaPath in defaultAreaPathes)
            {
                stistic.Append(areaPath + "\t");
                var total_p0 = autoStatSteps.GetNumTests(areaPath, tag, 0);
                var total_p1 = autoStatSteps.GetNumTests(areaPath, tag, 1);
                var total = total_p0 + total_p1;

                var auto_p0 = autoStatSteps.GetNumTests(areaPath, tag, 0, true);
                var auto_p1 = autoStatSteps.GetNumTests(areaPath, tag, 1, true);
                var auto_total = auto_p0 + auto_p1;

                stistic.Append(total + "\t" + auto_total + "\t");
                stistic.AppendLine();
            }

            stistic.AppendLine();

            Assert.Inconclusive(stistic.ToString());
        }
    }
}