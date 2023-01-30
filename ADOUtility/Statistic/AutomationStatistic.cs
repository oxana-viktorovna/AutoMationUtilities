using ADOCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using Statistic.Settings;
using Statistic.Steps;
using System;
using System.Collections.Generic;
using System.Linq;

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
            autoStatSteps = new AutomationStatisticSteps(adoSettings, autoStatSettings.DefaultAreaPathes, autoStatSettings.AsOf);
        }

        private AdoSettings adoSettings;
        private AutoStatisticSettings autoStatSettings;
        private AutomationStatisticSteps autoStatSteps;


        [TestMethod]
        public void GetAutoTestCoverage()
        {
            var otherPriorities = new List<int> { 2, 3, 4 };
            var rawStat = new List<(string priority, int autoCount, int allCount)>
            {
                ("0", autoStatSteps.GetAutomatedTestCountByPriority(0), autoStatSteps.GetTestCountByPriority(0)),
                ("1", autoStatSteps.GetAutomatedTestCountByPriority(1), autoStatSteps.GetTestCountByPriority(1)),
                ("2+", autoStatSteps.GetAutomatedTestCountByPriority(otherPriorities), autoStatSteps.GetTestCountByPriority(otherPriorities))
            };

            Assert.Inconclusive(Environment.NewLine + string.Join(Environment.NewLine,rawStat.Select(stat => $"priority {stat.priority} automated {stat.autoCount} all test cases {stat.allCount}")));

        }
    }
}