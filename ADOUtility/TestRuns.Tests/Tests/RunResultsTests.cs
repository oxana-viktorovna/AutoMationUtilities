using ADOCore;
using ADOCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.FileUtilities;
using SharedCore.Settings;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TestRuns.Steps;

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
        public void GetUiRunFailedResultsByBuild()
        {
            var testResults = apiSteps.GetTrxAttachements(testSettings.CurrBuildId, testSettings.BlockedTestRun);
            var uiFailedTests = testResults.GetFailedResults();

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var preBuildNum = apiSteps.GetBuildNumber(testSettings.PreviousBuildId);

            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}";
            fileSteps.SaveUiFailedResults(testSettings.SaveFolder, currFileName, uiFailedTests);

            var preFileName = $"{preBuildNum}{testSettings.PreviousRunPostffix}";
            fileSteps.CompareResultsWithPrevious(testSettings.SaveFolder, preFileName, currFileName);
        }

        [TestMethod]
        public void GetUiRunFailedResultsByRunIds()
        {
            var runIds = new List<int>() { 1997424 , 1997454, 1997524, 1997514, 1997418, 1997518, 1997436, 1997506, 1997426 };
            var testResults = apiSteps.GetTrxAttachements(runIds);
            var uiFailedTests = testResults.GetFailedResults();

            var currFileName = $"{testSettings.CurrRunPostffix}";
            fileSteps.SaveUiFailedResults(testSettings.SaveFolder, currFileName, uiFailedTests);
        }

        [TestMethod]
        public void GetUiRunBlockedFailedResults()
        {
            var testResults = apiSteps.GetTrxAttachementsSignleRun(testSettings.CurrBuildId, testSettings.BlockedTestRun);
            var uiFailedTests = testResults.GetFailedResults();

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var preBuildNum = apiSteps.GetBuildNumber(testSettings.PreviousBuildId);

            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}_Blocked";
            fileSteps.SaveUiFailedResults(testSettings.SaveFolder, currFileName, uiFailedTests);
            
            var preFileName = $"{preBuildNum}{testSettings.PreviousRunPostffix}_Blocked";
            fileSteps.CompareResultsWithPreviousIgnoreError(testSettings.SaveFolder, preFileName, currFileName);
        }

        [TestMethod]
        public void GetApiRunFailedResults()
        {
            var testResults = apiSteps.GetJUnitAttachements(testSettings.CurrBuildId);
            var failedResults = testResults.GetFailedTests();

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            fileSteps.SaveApiFailedResults(testSettings.SaveFolder, currBuildNum, failedResults);
        }

        [TestMethod]
        public void GetUiRunFailedResultsLocal()
        {
            var trx = File.ReadAllText(@"C:\Users\Aksana_Murashka\Downloads\USTrackerTasks_USBUILD01_2022-05-18_11_36_08.trx");
            var testResults = new List<TestRun> { XmlWorker.DeserializeXmlFromMemoryStream<TestRun>(trx) };
            var uiFailedTests = testResults.GetFailedResults();

            fileSteps.SaveUiFailedResults(testSettings.SaveFolder, "local", uiFailedTests);
        }

        [TestMethod]
        public void GetReRunString()
        {
            var testResults = apiSteps.GetTrxAttachements(testSettings.CurrBuildId);
            var uiFailedTests = testResults.GetFailedResults();

            var rerunString = new StringBuilder();
            rerunString.Append("&(");
            foreach (var uiFailedTest in uiFailedTests)
            {
                rerunString.Append($"Name~{uiFailedTest.testName}|");
            }
            rerunString.Remove(rerunString.Length-1, 1); // Remove last | symbol
            rerunString.Append(")");
        }
    }
}
