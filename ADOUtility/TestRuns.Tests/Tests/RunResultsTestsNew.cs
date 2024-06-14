using ADOCore;
using ADOCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using System.Collections.Generic;
using System.Linq;
using TestRuns.Models;
using TestRuns.Steps;
using TestRuns.Utilities;

namespace TestRuns.Tests
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
            apiStepsNew = new TestRunApiStepsNew(adoSettings);
            buildApiSteps = new BuildApiSteps(adoSettings);

            blockedPattern = "[b|B]locked";
        }

        private AdoSettings adoSettings;
        private TestRunTestSettings testSettings;
        private TestRunApiSteps apiSteps;
        private TestRunApiStepsNew apiStepsNew;
        private BuildApiSteps buildApiSteps;
        private string blockedPattern;


        public void CreateFullRunReport()
        {           
            var allBuildIds = buildApiSteps.GetAllBuildsIds(testSettings.CurrBuildId, testSettings.Reruns);
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
            var shortBuildName = buildApiSteps.GetShortBuildName(testSettings.CurrBuildId);
            var currFileName = $"{shortBuildName}{testSettings.CurrRunPostffix}";

            var reportBuilder = new RunNewReportBuilder(testSettings.SaveFolder, currFileName);
            var summaryReportBuilder = new RunNewSummaryBuilder(reportBuilder.Book);

            var totalNumOfRuns = reRunsUiSummary.Count + 1;
            summaryReportBuilder.CreateNewSummaryReport(testSettings.RunDuration);
            summaryReportBuilder.CreateUiSummary((testSettings.CurrBuildId, uiSummary), reRunsUiSummary);
            summaryReportBuilder.CreateApiSummary(totalNumOfRuns, (testSettings.CurrBuildId, apiSummary), reRunsApiSummary);
            summaryReportBuilder.CreateScriptSummary(totalNumOfRuns, (testSettings.CurrBuildId, apiSummary), reRunsApiSummary);
            summaryReportBuilder.CreateMainTotals(totalNumOfRuns);

            var uiReportBuilder = new RunNewUiSummaryBuilder(reportBuilder.Book);

            var allTestResultsNonBlocked = apiSteps.GetAllTrxRunResultsExcludeRun(allBuildIds, blockedPattern);
            var uiFailedTests = allTestResultsNonBlocked.GetNotPassedResults();

            var allTestResultsIncludeBlocked = apiSteps.GetAllTrxRunResultsIncludeRun(testSettings.CurrBuildIds, blockedPattern);
            var uiFailedBlockedTests = allTestResultsIncludeBlocked.GetNotPassedResults();
            var uiFailedBlockedTestsWithComments = apiSteps.CopyCommentsForBlocked(uiFailedBlockedTests, testSettings.SaveFolder);

            uiReportBuilder.CreateFullFailedUiReport(ResultReportConverter.Convert(uiFailedTests), uiFailedBlockedTestsWithComments);

            var apiReportBuilder = new RunNewApiSummaryBuilder(reportBuilder.Book);
            var apiTestResults = apiSteps.GetJUnitAttachements(testSettings.CurrBuildIds, testSettings.Reruns);
            var apiFailedResults = apiTestResults.GetFailedTests();
            apiReportBuilder.CreateFullFailedApiReport(apiFailedResults);

            reportBuilder.SaveReport();           
        }

        [TestMethod]
        public void CreateFullRunReportNew()
        {
            #region Get Run Summary

            var origRunBuilds = testSettings.CurrBuildIds.Select(build => (build, true));
            var reRunBuilds = testSettings.Reruns.Select(build => (build, false));
            var statistic = apiStepsNew.GetRunSummaryStat(origRunBuilds, reRunBuilds);
            statistic.Add((TestType.Script, (testSettings.CurrBuildIds[0], true), new RunSummary() { Passed = 1}));


            #endregion Get Run Summary

            #region Generate Report Summary

            var shortBuildName = buildApiSteps.GetShortBuildName(testSettings.CurrBuildIds);
            var currFileName = $"Full_{shortBuildName}{testSettings.CurrRunPostffix}";
            var reportBuilder = new RunNewReportBuilder(testSettings.SaveFolder, currFileName);
            var summaryReportBuilder = new RunNewSummaryBuilderNew(reportBuilder.Book, buildApiSteps);
            summaryReportBuilder.CreateSummaryReport(statistic, testSettings.RunDuration);

            #endregion Generate Report Summary
            
            #region Generate Failed Report

            var uiReportBuilder = new RunNewUiSummaryBuilder(reportBuilder.Book);

            var allTestResults = apiStepsNew.GetTrxAttachments(testSettings.CurrBuildIds, testSettings.Reruns);
            var uiFailedTests = allTestResults.GetNotPassedResults();

            var uiFailedTestsWithComments = apiStepsNew.CopyCommentsForBlocked(uiFailedTests, testSettings.SaveFolder);

            uiReportBuilder.CreateFullFailedUiReport(uiFailedTestsWithComments);
            /*
            var apiReportBuilder = new RunNewApiSummaryBuilder(reportBuilder.Book);
            var apiTestResults = apiStepsNew.GetJUnitAttachements(testSettings.CurrBuildIds, testSettings.Reruns);
            var apiFailedResults = apiTestResults.GetFailedTests();
            apiReportBuilder.CreateFullFailedApiReport(apiFailedResults);
            */
            #endregion Generate Failed Report
            
            reportBuilder.SaveReport();

        }
    }
}
