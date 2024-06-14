using ADOCore;
using ADOCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using System;
using TestRuns.Steps;
using TestRuns.Utilities;

namespace TestRuns.Tests
{
    [TestClass]
    public class TestPlanTests
    {
        private AdoSettings adoSettings;
        private TestPlanApiSteps apiSteps;
        private TestRunApiStepsNew testRunApiStepsNew;
        private TestRunTestSettings testSettings;

        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);
            apiSteps = new TestPlanApiSteps(adoSettings);
            testRunApiStepsNew = new TestRunApiStepsNew(adoSettings);
            var testSettingsReader = new SettingsReader("TestRunTestConfig.json");
            testSettings = new TestRunTestSettings(testSettingsReader);
        }

        [TestMethod]
        public void GetSuiteFailedTests()
        {
            var testPlanId = 199475;
            var suiteId = 271703;
            var testsIds = apiSteps.GetSuiteFailedTestIds(testPlanId, suiteId);

            Assert.Inconclusive(string.Join(",", testsIds));
        }

        [TestMethod]
        public void GetSuiteFailedTestResults()
        {
            var testPlanId = 199475;
            var suiteId = 271704;

            var failedRunIds = apiSteps.GetSuiteFailedTestRunIds(testPlanId, suiteId);
            var failedRunInfos = testRunApiStepsNew.GetRunInfo(failedRunIds);
            var failedTestsResults = testRunApiStepsNew.GetTrxAttachments(failedRunInfos).GetNotPassedResults();

            var reportBuilder = new RunNewReportBuilder(testSettings.SaveFolder, $"Results_TestPlan{testPlanId}TesSuite{suiteId}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}");
            var uiReportBuilder = new RunNewUiSummaryBuilder(reportBuilder.Book);
            uiReportBuilder.CreateFullFailedUiReport(ResultReportConverter.Convert(failedTestsResults));
            reportBuilder.SaveReport();
        }
    }
}
