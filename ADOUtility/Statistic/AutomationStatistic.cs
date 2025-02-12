using ADOCore;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using Statistic.Settings;
using Statistic.Steps;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        }

        private AdoSettings adoSettings;
        private AutoStatisticSettings autoStatSettings;
        private AutomationStatisticSteps autoStatSteps;
        private List<string> defaultAreaPathes;

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
                Environment.NewLine + string.Join(Environment.NewLine,rawStat.Select(stat => $"priority {stat.priority} automated {stat.autoCount} all test cases {stat.allCount}")));

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