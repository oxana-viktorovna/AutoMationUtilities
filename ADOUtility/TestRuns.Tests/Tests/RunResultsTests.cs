using ADOCore;
using ADOCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestRuns.Models;
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

            blockedPattern = "[b|B]locked";
        }

        private AdoSettings adoSettings;
        private TestRunTestSettings testSettings;
        private TestRunApiSteps apiSteps;
        private ReportFileSteps fileSteps;
        private string blockedPattern;

        [TestMethod]
        public void GetAllUiTestResultsByBuildId()
        {
            var allTestResults = apiSteps.GetAllTrxRunResults(testSettings.CurrBuildId);
            var uiPassedTests = allTestResults.GetPassedResults();
            var uiFailedTests = allTestResults.GetFailedResults();

            var allSelectedTestResults = new List<TestRunUnitTestResult>();
            allSelectedTestResults.AddRange(uiPassedTests);
            allSelectedTestResults.AddRange(uiFailedTests);

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}";

            fileSteps.SaveAllUiResults(testSettings.SaveFolder, currFileName, allSelectedTestResults);
        }

        [TestMethod]
        public void GetFailedUiRunResultsByBuild()
        {
            var allTestResultsExcludeBlocked = apiSteps.GetAllTrxRunResultsExcludeRun(testSettings.CurrBuildId, blockedPattern);
            var uiFailedTests = allTestResultsExcludeBlocked.GetFailedResults();

            var allTestResultsIncludeBlocked = apiSteps.GetAllTrxRunResultsIncludeRun(testSettings.CurrBuildId, blockedPattern);
            var uiFailedBlockedTests = allTestResultsIncludeBlocked.GetFailedResults();
            var uiFailedBlockedTestsWithComments = apiSteps.CopyCommentsForBlocked(uiFailedBlockedTests, testSettings.SaveFolder);

            var fullFailedTests = new List<ResultReport>();
            fullFailedTests.AddRange(ResultReportConverter.Convert(uiFailedTests));
            fullFailedTests.AddRange(uiFailedBlockedTestsWithComments);

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}";
            fileSteps.SaveUiResults(testSettings.SaveFolder, currFileName, fullFailedTests);
        }

        [TestMethod]
        public void GetFailedUiRunResultsByBuilds()
        {
            var allBuildIds = apiSteps.GetAllBuildsIds(testSettings.CurrBuildId, testSettings.Reruns);

            var allTestResultsExcludeBlocked = apiSteps.GetAllTrxRunResultsExcludeRun(allBuildIds, blockedPattern);
            var uiFailedTests = allTestResultsExcludeBlocked.GetFailedResults();

            var allTestResultsIncludeBlocked = apiSteps.GetAllTrxRunResultsIncludeRun(testSettings.CurrBuildId, blockedPattern);
            var uiFailedBlockedTests = allTestResultsIncludeBlocked.GetFailedResults();
            var uiFailedBlockedTestsWithComments = apiSteps.CopyCommentsForBlocked(uiFailedBlockedTests, testSettings.SaveFolder);

            var fullFailedTests = new List<ResultReport>();
            fullFailedTests.AddRange(ResultReportConverter.Convert(uiFailedTests));
            fullFailedTests.AddRange(uiFailedBlockedTestsWithComments);

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}";
            fileSteps.SaveUiResults(testSettings.SaveFolder, currFileName, fullFailedTests);
        }

        [TestMethod]
        public void GetFailedUiBlockedRunResults()
        {
            var allTestResultsIncludeBlocked = apiSteps.GetAllTrxRunResultsIncludeRun(testSettings.CurrBuildId, blockedPattern);
            var uiFailedBlockedTests = allTestResultsIncludeBlocked.GetFailedResults();
            var uiFailedBlockedTestsWithComments = apiSteps.CopyCommentsForBlocked(uiFailedBlockedTests, testSettings.SaveFolder);

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}_FailedBlocked";
            fileSteps.SaveUiResults(testSettings.SaveFolder, currFileName, uiFailedBlockedTestsWithComments);
        }

        [TestMethod]
        public void GetPassedUiBlockedRunResults()
        {
            var allTestResultsIncludeBlocked = apiSteps.GetAllTrxRunResultsIncludeRun(testSettings.CurrBuildId, blockedPattern);
            var uiPassedBlockedTests = allTestResultsIncludeBlocked.GetPassedResults();
            var uiPassedBlockedTestsWithComments = apiSteps.CopyCommentsForBlocked(uiPassedBlockedTests, testSettings.SaveFolder);

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}_PassedBlocked";
            fileSteps.SaveUiResults(testSettings.SaveFolder, currFileName, uiPassedBlockedTestsWithComments);
        }

        [TestMethod]
        public void GetFailedApiRunResults()
        {
            var testResults = apiSteps.GetJUnitAttachements(testSettings.CurrBuildId);
            var failedResults = testResults.GetFailedTests();

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            fileSteps.SaveApiFailedResults(testSettings.SaveFolder, currBuildNum, failedResults);
        }

        [TestMethod]
        public void GetReRunString()
        {
            var allTestResults = apiSteps.GetAllTrxRunResultsExcludeRun(testSettings.CurrBuildId, blockedPattern);
            var uiFailedTests = allTestResults.GetFailedResults();

            var rerunString = new StringBuilder();
            rerunString.Append("&(");
            foreach (var uiFailedTest in uiFailedTests)
            {
                rerunString.Append($"Name~{uiFailedTest.testName}|");
            }
            rerunString.Remove(rerunString.Length - 1, 1); // Remove last | symbol
            rerunString.Append(')');

            Assert.Inconclusive(rerunString.ToString());
        }

        [TestMethod]
        public void GetUiRunDurationCompare()
        {
            var currAllTestResults = apiSteps.GetAllTrxRunResults(testSettings.CurrBuildId);
            var currUiPassedTests = currAllTestResults.GetPassedResults();
            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);

            var preAllTestResults = apiSteps.GetAllTrxRunResults(testSettings.PreviousBuildId);
            var preUiPassedTests = preAllTestResults.GetPassedResults();
            var preUiTestNames = preUiPassedTests.Select(t => t.testName);
            var preBuildNum = apiSteps.GetBuildNumber(testSettings.PreviousBuildId);

            List<(TestRunUnitTestResult currResult, DateTime preDuration)> dureactionCompare = new();
            foreach (var currUiPassedTest in currUiPassedTests)
            {
                if (preUiTestNames.Contains(currUiPassedTest.testName))
                {
                    var preUiPassedTest = preUiPassedTests.FirstOrDefault(t => t.testName == currUiPassedTest.testName);
                    dureactionCompare.Add((currUiPassedTest, preUiPassedTest.duration));
                }
            }

            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}_DurationCompare";
            fileSteps.SaveUiPassedResultsWithDurationComapre(testSettings.SaveFolder, currFileName, dureactionCompare, preBuildNum);
        }

        [TestMethod]
        public void GetUiRunDuration()
        {
            var allTestResults = apiSteps.GetAllTrxRunResults(testSettings.CurrBuildId);
            var uiPassedTests = allTestResults.GetPassedResults();

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}_Duration";

            fileSteps.SaveUiPassedResultsWithDuration(testSettings.SaveFolder, currFileName, uiPassedTests);
        }
    }
}
