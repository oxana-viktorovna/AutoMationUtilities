using ADOCore;
using ADOCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            apiStepsNew = new TestRunApiStepsNew(adoSettings);
            buildApiSteps = new BuildApiSteps(adoSettings);
            fileSteps = new ReportFileSteps();

            blockedPattern = "[b|B]locked";
        }

        private AdoSettings adoSettings;
        private TestRunTestSettings testSettings;
        private TestRunApiSteps apiSteps;
        private TestRunApiStepsNew apiStepsNew;
        private BuildApiSteps buildApiSteps;
        private ReportFileSteps fileSteps;
        private string blockedPattern;

        [TestMethod]
        public void GetAllUiTestResultsByBuildId()
        {
            var allTestResults = apiSteps.GetAllTrxRunResults(testSettings.CurrBuildIds);
            var uiPassedTests = allTestResults.GetPassedResults();
            var uiFailedTests = allTestResults.GetFailedResults();

            var allSelectedTestResults = new List<TestRunUnitTestResult>();
            allSelectedTestResults.AddRange(uiPassedTests);
            allSelectedTestResults.AddRange(uiFailedTests);

            var shortBuildName = buildApiSteps.GetShortBuildName(testSettings.CurrBuildIds);
            var currFileName = $"{shortBuildName}{testSettings.CurrRunPostffix}";

            fileSteps.SaveAllUiResults(testSettings.SaveFolder, currFileName, allSelectedTestResults);
        }

        [TestMethod]
        public void GetPassedOnReRunUiRunResultsByBuild()
        {
            var allTestResultsExcludeBlocked = apiSteps.GetAllTrxRunResultsExcludeRun(testSettings.CurrBuildIds, blockedPattern);
            var uiPassedOnReRunTests = allTestResultsExcludeBlocked.GetPassedOnReRunResults();

            var shortBuildName = buildApiSteps.GetShortBuildName(testSettings.CurrBuildIds);
            var currFileName = $"{shortBuildName}{testSettings.CurrRunPostffix}";
            fileSteps.SaveUiResults(testSettings.SaveFolder, currFileName, ResultReportConverter.Convert(uiPassedOnReRunTests));
        }

        [TestMethod]
        public void GetFailedUiRunResultsByBuild()
        {
            var shortBuildName = buildApiSteps.GetBuildName(testSettings.CurrBuildIds);
            var currFileName = $"{shortBuildName}{testSettings.CurrRunPostffix}";
            var reportBuilder = new RunNewReportBuilder(testSettings.SaveFolder, currFileName);
            reportBuilder.DfltFileName = "FailedUI";
            var uiReportBuilder = new RunNewUiSummaryBuilder(reportBuilder.Book);

            var allTestResults = apiStepsNew.GetTrxAttachments(testSettings.CurrBuildIds, testSettings.Reruns);
            var uiFailedTests = allTestResults.GetFailedResults();

            var uiFailedTestsWithComments = apiStepsNew.CopyCommentsForBlocked(uiFailedTests, testSettings.SaveFolder);

            uiReportBuilder.CreateFullFailedUiReport(uiFailedTestsWithComments);

            reportBuilder.SaveReport();
        }

        [TestMethod]
        public void GetFailedUiRunResultsByBuildMulti()
        {
            var allTestResultsExcludeBlocked = apiSteps.GetAllTrxRunResultsExcludeRun(testSettings.CurrBuildIds, blockedPattern);
            var uiFailedTests = allTestResultsExcludeBlocked.GetFailedResults();

            var allTestResultsIncludeBlocked = apiSteps.GetAllTrxRunResultsIncludeRun(testSettings.CurrBuildIds, blockedPattern);
            var uiFailedBlockedTests = allTestResultsIncludeBlocked.GetFailedResults();
            var uiFailedBlockedTestsWithComments = apiSteps.CopyCommentsForBlocked(uiFailedBlockedTests, testSettings.SaveFolder);

            var fullFailedTests = new List<ResultReport>();
            fullFailedTests.AddRange(ResultReportConverter.Convert(uiFailedTests));
            if (uiFailedBlockedTestsWithComments != null)
                fullFailedTests.AddRange(uiFailedBlockedTestsWithComments);

            var shortBuildName = buildApiSteps.GetShortBuildName(testSettings.CurrBuildIds);
            var currFileName = $"{shortBuildName}{testSettings.CurrRunPostffix}";
            fileSteps.SaveUiResults(testSettings.SaveFolder, currFileName, fullFailedTests);
        }

        [TestMethod]
        public void GetFailedUiBlockedRunResults()
        {
            var allTestResultsIncludeBlocked = apiSteps.GetAllTrxRunResultsIncludeRun(testSettings.CurrBuildIds, blockedPattern);
            var uiFailedBlockedTests = allTestResultsIncludeBlocked.GetFailedResults();
            var uiFailedBlockedTestsWithComments = apiSteps.CopyCommentsForBlocked(uiFailedBlockedTests, testSettings.SaveFolder);

            var shortBuildName = buildApiSteps.GetShortBuildName(testSettings.CurrBuildIds);
            var currFileName = $"{shortBuildName}{testSettings.CurrRunPostffix}_FailedBlocked";
            fileSteps.SaveUiResults(testSettings.SaveFolder, currFileName, uiFailedBlockedTestsWithComments);
        }

        [TestMethod]
        public void GetPassedUiBlockedRunResults()
        {
            var allTestResultsIncludeBlocked = apiSteps.GetAllTrxRunResultsIncludeRun(testSettings.CurrBuildIds, blockedPattern);
            var uiPassedBlockedTests = allTestResultsIncludeBlocked.GetPassedResults();
            var uiPassedBlockedTestsWithComments = apiSteps.CopyCommentsForBlocked(uiPassedBlockedTests, testSettings.SaveFolder);

            var shortBuildName = buildApiSteps.GetShortBuildName(testSettings.CurrBuildIds);
            var currFileName = $"{shortBuildName}{testSettings.CurrRunPostffix}_PassedBlocked";
            fileSteps.SaveUiResults(testSettings.SaveFolder, currFileName, uiPassedBlockedTestsWithComments);
        }

        [TestMethod]
        public void GetFailedApiRunResults()
        {
            var testResults = apiSteps.GetJUnitAttachements(testSettings.CurrBuildIds);
            var failedResults = testResults.GetFailedTests();

            var shortBuildName = buildApiSteps.GetShortBuildName(testSettings.CurrBuildIds);
            fileSteps.SaveApiFailedResults(testSettings.SaveFolder, shortBuildName, failedResults);
        }

        [TestMethod]
        public void GetReRunString()
        {
            var allTestResults = apiStepsNew.GetTrxAttachments(testSettings.CurrBuildIds, testSettings.Reruns);
            var uiFailedTests = allTestResults.GetFailedResults().ExcludeBlocked();

            var groupedByEnv = uiFailedTests.GroupBy(r => r.Env);

            var rerunStrings = new List<string>();
            foreach (var envGroup in groupedByEnv)
            {
                var rerunString = new StringBuilder();
                rerunString.Append("&(");
                foreach (var test in envGroup)
                {
                    Regex.Replace(test.testName, @"\((.*?)\)", "");
                    rerunString.Append($"Name~{test.testName}|");
                }
                rerunString.Remove(rerunString.Length - 1, 1); // Remove last | symbol
                rerunString.Append(')');


                rerunStrings.Add($"{envGroup.Key} {envGroup.Count()}{Environment.NewLine}{rerunString}");
            }

            Assert.Inconclusive(string.Join(Environment.NewLine, rerunStrings));
        }

        [TestMethod]
        public void GetUiRunDurationCompare()
        {
            var currAllTestResults = apiSteps.GetAllTrxRunResults(testSettings.CurrBuildId);
            var currUiPassedTests = currAllTestResults.GetPassedResults();
            var currBuildNum = buildApiSteps.GetBuildNumber(testSettings.CurrBuildId);

            var preAllTestResults = apiSteps.GetAllTrxRunResults(testSettings.PreviousBuildId);
            var preUiPassedTests = preAllTestResults.GetPassedResults();
            var preUiTestNames = preUiPassedTests.Select(t => t.testName);
            var preBuildNum = buildApiSteps.GetBuildNumber(testSettings.PreviousBuildId);

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

            var shortBuildName = buildApiSteps.GetShortBuildName(testSettings.CurrBuildIds);
            var currFileName = $"{shortBuildName}{testSettings.CurrRunPostffix}_Duration";

            fileSteps.SaveUiPassedResultsWithDuration(testSettings.SaveFolder, currFileName, uiPassedTests);
        }
    }
}
