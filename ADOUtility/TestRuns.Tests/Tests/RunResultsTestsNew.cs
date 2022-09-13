using ADOCore;
using ADOCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using System.Collections.Generic;
using TestRuns.Models;
using TestRuns.Steps;
using TestRuns.Utilities;

namespace TestRuns
{
    [TestClass]
    public class RunResultsTestsNew
    {
        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);

            var testSettingsReader = new SettingsReader("TestRunTestConfig.json");
            testSettings = new TestRunTestSettings(testSettingsReader);
            apiSteps = new TestRunApiSteps(adoSettings);

            blockedPattern = "[b|B]locked";
        }

        private AdoSettings adoSettings;
        private TestRunTestSettings testSettings;
        private TestRunApiSteps apiSteps;
        private string blockedPattern;

        [TestMethod]
        public void CreateFullRunReport()
        {           
            var allBuildIds = apiSteps.GetAllBuildsIds(testSettings.CurrBuildId, testSettings.Reruns);
            var mainStatistic = apiSteps.GetRunStatistics(testSettings.CurrBuildId);
            var uiSummary = mainStatistic.GetUiStatistic();
            var apiSummary = mainStatistic.GetApiStatistic();

            var reRunsUiSummary = new List<(int, RunSummary)>();
            var reRunsApiSummary = new List<(int, RunSummary)>();
            foreach (var rerunBuildId in testSettings.Reruns)
            {
                var reRunStatistic = apiSteps.GetRunStatistics(rerunBuildId);
                var reRunUiSummary = reRunStatistic.GetUiStatistic();
                reRunsUiSummary.Add((rerunBuildId, reRunUiSummary));
                var reRunApiSummary = reRunStatistic.GetApiStatistic();
                reRunsApiSummary.Add((rerunBuildId, reRunApiSummary));
            }
            var currBuildNum = apiSteps.GetBuildNumber(testSettings.CurrBuildId);
            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}";

            var reportBuilder = new RunNewReportBuilder(testSettings.SaveFolder, currFileName);
            var summaryReportBuilder = new RunNewSummaryBuilder(reportBuilder.Book);

            var totalNumOfRuns = reRunsUiSummary.Count + 1;
            summaryReportBuilder.CreateNewSummaryReport(testSettings.RunDuration);
            summaryReportBuilder.CreateUiSummary((testSettings.CurrBuildId, uiSummary), reRunsUiSummary);
            summaryReportBuilder.CreateApiSummary(totalNumOfRuns, (testSettings.CurrBuildId, apiSummary), reRunsApiSummary);
            summaryReportBuilder.CreateScriptSummary(totalNumOfRuns, (testSettings.CurrBuildId, apiSummary), reRunsApiSummary);
            summaryReportBuilder.CreateMainTotals(totalNumOfRuns);

            var uiReportBuilder = new RunNewUiSummaryBuilder(reportBuilder.Book);

            var allTestResultsExcludeBlocked = apiSteps.GetAllTrxRunResultsExcludeRun(allBuildIds, blockedPattern);
            var uiFailedTests = allTestResultsExcludeBlocked.GetFailedResults();

            var allTestResultsIncludeBlocked = apiSteps.GetAllTrxRunResultsIncludeRun(testSettings.CurrBuildId, blockedPattern);
            var uiFailedBlockedTests = allTestResultsIncludeBlocked.GetFailedResults();
            var uiFailedBlockedTestsWithComments = apiSteps.CopyCommentsForBlocked(uiFailedBlockedTests, testSettings.SaveFolder);

            uiReportBuilder.CreateFullFailedUiReport(ResultReportConverter.Convert(uiFailedTests), uiFailedBlockedTestsWithComments);

            var apiReportBuilder = new RunNewApiSummaryBuilder(reportBuilder.Book);
            var apiTestResults = apiSteps.GetJUnitAttachements(testSettings.CurrBuildId, testSettings.Reruns);
            var apiFailedResults = apiTestResults.GetFailedTests();
            apiReportBuilder.CreateFullFailedApiReport(apiFailedResults);

            reportBuilder.SaveReport();           
        }
    }
}
