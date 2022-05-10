﻿using ADOCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using TestRuns.Steps;
using TestRuns.Utilities;

namespace TestRuns
{
    [TestClass]
    public class RunResultsSummary
    {
        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);

            var testSettingsReader = new SettingsReader("TestRunTestConfig.json");
            testSettings = new TestRunTestSettings(testSettingsReader);
            apiSteps = new TestRunApiSteps(adoSettings);
            excelWorker = new RunSummaryWorker();
        }

        private AdoSettings adoSettings;
        private TestRunTestSettings testSettings;
        private TestRunApiSteps apiSteps;
        private RunSummaryWorker excelWorker;

        [TestMethod]
        public void GetRunStatistic()
        {
            var statistic = apiSteps.GetRunStatistics(testSettings.CurrBuildId);
            var uiSummary = statistic.GetUiStatistic();
            var apiSummary = statistic.GetApiStatistic();

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var preBuildNum = apiSteps.GetBuildNumber(testSettings.PreviousBuildId);

            excelWorker.UpdateCurrentSummaryReport(testSettings.SaveFolder, currBuildNum, uiSummary, apiSummary, testSettings.RunDuration);
            excelWorker.UpdatePreviousSummaryReport(testSettings.SaveFolder, currBuildNum, preBuildNum);
        }

    }
}
