using ADOCore;
using ADOCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using TestRuns.Steps;
using TestRuns.Utilities;

namespace TestRuns
{
    [TestClass]
    public class RunResultsTests
    {
        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);

            var testSettingsReader = new SettingsReader("TestRunTestConfig.json");
            testSettings = new TestRunTestSettings(testSettingsReader);
            apiSteps = new TestRunApiSteps(adoSettings);
            fileSteps = new ReportFileSteps();
        }

        private AdoSettings adoSettings;
        private TestRunTestSettings testSettings;
        private TestRunApiSteps apiSteps;
        private ReportFileSteps fileSteps;

        [TestMethod]
        public void GetUiRunFailedResults()
        {
            var testResults = apiSteps.GetTrxAttachements(testSettings.CurrBuildId);
            var uiFailedTests = testResults.GetFailedResults();

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var preBuildNum = apiSteps.GetBuildNumber(testSettings.PreviousBuildId);

            fileSteps.SaveUiFailedResults(testSettings.SaveFolder, currBuildNum, uiFailedTests);
            fileSteps.CompareResultsWithPrevious(testSettings.SaveFolder, preBuildNum, currBuildNum);
        }

        [TestMethod]
        public void GetApiRunFailedResults()
        {
            var testResults = apiSteps.GetJUnitAttachements(testSettings.CurrBuildId);
            var failedResults = testResults.GetFailedTests();

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            fileSteps.SaveApiFailedResults(testSettings.SaveFolder, currBuildNum, failedResults);
        }
    }
}
