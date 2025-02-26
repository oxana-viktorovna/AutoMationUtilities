using ADOCore;
using ADOCore.Models.WiqlQuery;
using ADOCore.Steps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.FileUtilities;
using SharedCore.Settings;
using Statistic.Settings;
using Statistic.Steps;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Text;

namespace Statistic
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
            workItemApiSteps = new WorkItemApiSteps(adoSettings);
        }

        private AdoSettings adoSettings;
        private AutoStatisticSettings autoStatSettings;
        private AutomationStatisticSteps autoStatSteps;
        private WorkItemApiSteps workItemApiSteps;
        private List<string> defaultAreaPathes;

        [TestMethod]
        public void GetNewFeatureAutomationStat()
        {
            var wiqlSteps = new WiqlQuerySteps(adoSettings);
            var pbis_withIncorrectAdoSet = new List<int> {266895};

            var query = new WiqlDirectLinksQueryBuilder().AddAttributesToGet("[System.Id]")
                .AddSourceCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Feature")
                .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.In, "('Done', 'Removed', 'In Design')")
                .AddSourceCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("IterationPath"), WiqlConsnt.Operator.Under, "Tracker\\2025")
                .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, "System.Tags", WiqlConsnt.Operator.Contains, "on hold")
                .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, "System.Tags", WiqlConsnt.Operator.Contains, "On-Hold")
                .AddSourceCondition(WiqlConsnt.Conjunction.AndNot, "System.Tags", WiqlConsnt.Operator.Contains, "no release")
                .AddSourceCondition(WiqlConsnt.Conjunction.Or, "[System.Id]", WiqlConsnt.Operator.In, "("+string.Join(',', pbis_withIncorrectAdoSet)+")")
                .AddTargetCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Product Backlog Item")
                .AddTargetCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.In, "('Removed', 'New', 'Approved')")
                .Build();
            var pbis = wiqlSteps.GetLinkedItems(query);
            var pbiIds = pbis.Select(pbi => pbi.target.id);

            query = new WiqlDirectLinksQueryBuilder().AddAttributesToGet("[System.Id]")
                .AddSourceCondition(null, "[System.Id]", WiqlConsnt.Operator.In, "(" + string.Join(',', pbiIds) + ")")
                .AddTargetCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Task")
                .AddTargetCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.NotEqual, "Removed")
                .Build();
            var pbiTasks = wiqlSteps.GetLinkedItems(query);
            var pbiTaskIds = pbiTasks.Select(pbit => pbit.target.id);
            var allIParentds = pbiTaskIds.Concat(pbiIds).ToList();

            query = new WiqlDirectLinksQueryBuilder().AddAttributesToGet("[System.Id]")
                .AddSourceCondition(null, "[System.Id]", WiqlConsnt.Operator.In, "(" + string.Join(',', allIParentds) + ")")
                .AddTargetCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Test Case")
                .AddTargetCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomationStatus"), WiqlConsnt.Operator.NotEqual, "Automated")
                .AddTargetCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AutomationPriority"), WiqlConsnt.Operator.NotEqual, 4)
                .AddTargetCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("Priority"), WiqlConsnt.Operator.In, "(0,1)")
                .AddTargetCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.NotEqual, "Closed")
                .Build();
            var tests = wiqlSteps.GetLinkedItems(query);

            var content = new StringBuilder();
            content.AppendLine("Feature Id,Feature Name,PBI Id,Task Id,Area Path,Test Id,Test Name,Test Priority,Test Iteration Path");
            foreach (var test in tests)
            {
                var task = pbiTasks.FirstOrDefault(i => i.target.id == test.source.id);
                var taskId = task == null ? 0 : task.target.id;
                var pbi = task == null
                    ? pbis.FirstOrDefault(i => i.target.id == test.source.id)
                    : pbis.FirstOrDefault(i => i.target.id == task.source.id);

                var pbi_item = workItemApiSteps.GetWorkItem(pbi.target.id);
                var feature_item = workItemApiSteps.GetWorkItem(pbi.source.id);
                var test_item = workItemApiSteps.GetWorkItem(test.target.id);
                content.AppendLine($"{pbi.source.id},{feature_item.fields.Title},{pbi.target.id},{taskId},{pbi_item.fields.AreaPath},{test.target.id},{test_item.fields.Title.Replace(",", "_")},{test_item.fields.Priority},{test_item.fields.IterationPath}");
            }

            var currentDate = DateTime.Now.ToString("dd-MM-yy");
            var fileName = $"NewFeaturesTests_{currentDate}.csv";
            var filePath = Path.Combine(autoStatSettings.SaveFolder, fileName);
            var csv = new CsvWorker(filePath);
            csv.Write(content);
        }

        [TestMethod]
        public void GetUIAutoTestCoverage()
        {
            var otherPriorities = new List<int> { 2, 3, 4 };
            var rawStat = new List<(string priority, int autoCount, int allCount)>
            {
                ("0", autoStatSteps.GetAutomatedTestCountByPriority(0), autoStatSteps.GetTestCountByPriority(0)),
                ("1", autoStatSteps.GetAutomatedTestCountByPriority(1), autoStatSteps.GetTestCountByPriority(1)),
                ("2+", autoStatSteps.GetAutomatedTestCountByPriority(new List<int> { 2, 3, 4 }), autoStatSteps.GetTestCountByPriority(2) + autoStatSteps.GetTestCountByPriority(new List<int> { 3, 4 }))
            };

            Assert.Inconclusive(Environment.NewLine + "All automated " + rawStat.Sum(stat => stat.autoCount) + " All test cases " + rawStat.Sum(stat => stat.allCount) +
                Environment.NewLine + string.Join(Environment.NewLine, rawStat.Select(stat => $"priority {stat.priority} automated {stat.autoCount} all test cases {stat.allCount}")));

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