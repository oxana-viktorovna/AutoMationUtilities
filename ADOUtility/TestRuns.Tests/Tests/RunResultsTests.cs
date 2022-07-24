using ADOCore;
using ADOCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.FileUtilities;
using SharedCore.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestRuns.Models;
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

            blockedPattern = "[b|B]locked";
        }

        private AdoSettings adoSettings;
        private TestRunTestSettings testSettings;
        private TestRunApiSteps apiSteps;
        private ReportFileSteps fileSteps;
        private string blockedPattern;

        [TestMethod]
        public void GetUiRunFailedResultsByBuild()
        {
            var testResults = apiSteps.GetTrxAttachementsExcludeRun(testSettings.CurrBuildId, blockedPattern, Outcome.Failed);
            var uiFailedTests = testResults.GetFailedResults();

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var preBuildNum = apiSteps.GetBuildNumber(testSettings.PreviousBuildId);

            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}";
            fileSteps.SaveUiResults(testSettings.SaveFolder, currFileName, uiFailedTests);

            var preFileName = $"{preBuildNum}{testSettings.PreviousRunPostffix}";
            fileSteps.CompareResultsWithPrevious(testSettings.SaveFolder, preFileName, currFileName);
        }

        [TestMethod]
        public void GetUiRunBlockedFailedResults()
        {
            var testResults = apiSteps.GetTrxAttachementsSignleRun(testSettings.CurrBuildId, blockedPattern, Outcome.Failed);
            var uiFailedTests = testResults.GetFailedResults();

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var preBuildNum = apiSteps.GetBuildNumber(testSettings.PreviousBuildId);

            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}_Blocked";
            fileSteps.SaveUiResults(testSettings.SaveFolder, currFileName, uiFailedTests);

            var preFileName = $"{preBuildNum}{testSettings.PreviousRunPostffix}_Blocked";
            fileSteps.CompareResultsWithPreviousIgnoreError(testSettings.SaveFolder, preFileName, currFileName);
        }

        [TestMethod]
        public void GetUiRunBlockedPassedResults()
        {
            var currTestResults = apiSteps.GetTrxAttachementsSignleRun(testSettings.CurrBuildId, blockedPattern, Outcome.Passed);
            var currUiPassedTests = currTestResults.GetPassedResults();

            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);

            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}_PassedBlocked";
            fileSteps.SaveUiResults(testSettings.SaveFolder, currFileName, currUiPassedTests, false);
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
        public void GetReRunString()
        {
            var build = testSettings.CurrBuildId;
            //var build = testSettings.Reruns[0];
            var testResults = apiSteps.GetTrxAttachementsExcludeRun(build, blockedPattern, Outcome.Failed);
            var uiFailedTests = testResults.GetFailedResults();

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
            var currTestResults = apiSteps.GetTrxAttachements(testSettings.CurrBuildId, Outcome.Passed);
            var currUiPassedTests = currTestResults.GetPassedResults();
            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);

            var preTestResults = apiSteps.GetTrxAttachements(testSettings.PreviousBuildId, Outcome.Passed);
            var preUiPassedTests = preTestResults.GetPassedResults();
            var preBuildNum = apiSteps.GetBuildNumber(testSettings.PreviousBuildId);
            var preTestNames = preUiPassedTests.Select(t => t.testName);

            List<(TestRunUnitTestResult currResult, DateTime preDuration)> dureactionCompare = new();
            foreach (var currUiPassedTest in currUiPassedTests)
            {
                if (preTestNames.Contains(currUiPassedTest.testName))
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
            var currTestResults = apiSteps.GetTrxAttachements(testSettings.CurrBuildId, Outcome.Passed);
            var currUiPassedTests = currTestResults.GetPassedResults();
            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);

            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}_Duration";
            fileSteps.SaveUiPassedResultsWithDuration(testSettings.SaveFolder, currFileName, currUiPassedTests);
        }
    }
}
