using ADOCore;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula.Functions;
using SharedCore.FileUtilities;
using SharedCore.Settings;
using Statistic.Settings;
using Statistic.Steps;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TestRuns.Steps;

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
            var pbis = autoStatSteps.GetLinkedItems(@"SELECT
    [System.Id]
FROM workitemLinks
WHERE
    (
        [Source].[System.TeamProject] = @project
        AND [Source].[System.WorkItemType] = 'Feature'
        AND NOT [Source].[System.State] IN ('Done', 'Removed')
        AND [Source].[System.IterationPath] UNDER 'Tracker\2025'
        AND NOT [Source].[System.Tags] CONTAINS 'on hold'
        AND NOT [Source].[System.Tags] CONTAINS 'On-Hold'
        AND NOT [Source].[System.Tags] CONTAINS 'no release'
        AND [Source].[System.State] <> 'In Design'
    )
    AND (
        [Target].[System.TeamProject] = @project
        AND [Target].[System.WorkItemType] = 'Product Backlog Item'
        AND NOT [Target].[System.State] IN ('Removed', 'New', 'Approved')
    )
ORDER BY [System.AreaPath],
    [System.Id]
MODE (MustContain)",
"System.LinkTypes.Hierarchy-Forward");
            var pbiIds = pbis.Select(pbi => pbi.target.id);

            var pbiTasks = autoStatSteps.GetLinkedItems($@"SELECT
    [System.Id],
    [System.WorkItemType],
    [System.Title],
    [System.AssignedTo],
    [System.State],
    [System.Tags]
FROM workitemLinks
WHERE
    (
        [Source].[System.TeamProject] = @project
        AND [Source].[System.Id] IN ({string.Join(',', pbiIds)})
    )
    AND (
        [Target].[System.TeamProject] = @project
        AND [Target].[System.WorkItemType] = 'Task'
        AND [Target].[System.State] <> 'Removed'
    )
ORDER BY [System.Id]
MODE (MustContain)
", "System.LinkTypes.Hierarchy-Forward");
            var pbiTaskIds = pbiTasks.Select(pbit => pbit.target.id);

            var allIds = pbiTaskIds.Concat(pbiIds).ToList();

            var tests = autoStatSteps.GetLinkedItems($@"SELECT
    [System.Id]
FROM workitemLinks
WHERE
    (
        [Source].[System.TeamProject] = @project
        AND NOT [Source].[System.State] IN ('Removed', 'New', 'Approved')
        AND [Source].[System.Id] IN ({string.Join(',', allIds)})
    )
    AND (
        [Target].[System.TeamProject] = @project
        AND [Target].[System.WorkItemType] = 'Test Case'
        AND [Target].[Microsoft.VSTS.TCM.AutomationStatus] <> 'Automated'
        AND [Target].[TR.Elite.AutomationPriority] <> 4
        AND [Target].[Microsoft.VSTS.Common.Priority] IN (0, 1)
        AND [Target].[System.State] <> 'Closed'
    )
ORDER BY [System.Id]
MODE (MustContain)",
"Microsoft.VSTS.Common.TestedBy-Forward");

            var content = new StringBuilder();
            content.AppendLine("Feature Id,Feature Name,PBI Id,Task Id,Area Path,Test Id,Test Name,Test Priority,Test Iteration Path");
            foreach (var test in tests)
            {
                var task = pbiTasks.FirstOrDefault(i => i.target.id == test.source.id);
                var taskId = task == null ? 0 : task.target.id;
                var pbi = task == null
                    ? pbis.FirstOrDefault(i => i.target.id == test.source.id)
                    : pbis.FirstOrDefault(i => i.target.id == task.source.id);

                var pbi_item = workItemApiSteps.GetWorkItemNew(pbi.target.id);
                var feature_item = workItemApiSteps.GetWorkItemNew(pbi.source.id);
                var test_item = workItemApiSteps.GetWorkItemNew(test.target.id);
                content.AppendLine($"{pbi.source.id},{feature_item.fields.SystemTitle},{pbi.target.id},{taskId},{pbi_item.fields.SystemAreaPath},{test.target.id},{test_item.fields.SystemTitle.Replace(",", "_")},{test_item.fields.MicrosoftVSTSCommonPriority},{test_item.fields.SystemIterationPath}");
            }

            var folder = "C:\\Users\\Aksana_Murashka\\Documents\\TRI-SRTR\\TestResultsAnalyse";
            var currentDate = DateTime.Now.ToString("dd-MM-yy");
            var fileName = $"NewFeaturesTests_{currentDate}.csv";
            var filePath = Path.Combine(folder, fileName);
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
            var stistic = new StringBuilder("AreaPath \t total \t auto");
            stistic.AppendLine();
            foreach (var areaPath in defaultAreaPathes)
            {
                stistic.Append(areaPath + "\t");
                var total_p0 = autoStatSteps.GetNumTests(areaPath, "255scope", 0);
                var total_p1 = autoStatSteps.GetNumTests(areaPath, "255scope", 1);
                var total = total_p0 + total_p1;

                var auto_p0 = autoStatSteps.GetNumTests(areaPath, "255scope", 0, true);
                var auto_p1 = autoStatSteps.GetNumTests(areaPath, "255scope", 1, true);
                var auto_total = auto_p0 + auto_p1;

                stistic.Append(total + "\t" + auto_total + "\t");
                stistic.AppendLine();
            }

            stistic.AppendLine();

            Assert.Inconclusive(stistic.ToString());
        }
    }
}