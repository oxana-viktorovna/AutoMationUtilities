using ADOCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using Statistic.Settings;
using Statistic.Steps;
using Statistic.Utilities;
using System.Collections.Generic;

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

            var readPath = PathResolver.GetReadPath(autoStatSettings.FirstTimeRun, autoStatSettings.SaveFolder);
            var savePath = PathResolver.GetSavePath(autoStatSettings.SaveFolder);
            var excel = new ExcelAutoStatWorker(readPath);
            excel.PopulateAutoStat(savePath, rawStat, autoStatSettings.AsOf);
        }
    }
}