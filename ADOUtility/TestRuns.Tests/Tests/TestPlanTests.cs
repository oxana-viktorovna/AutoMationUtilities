﻿using ADOCore;
using ADOCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var suiteId = 281104;
            var testsIds = apiSteps.GetSuiteNotPassedTestIds(testPlanId, suiteId);

            Assert.Inconclusive(string.Join(",", testsIds));
        }

        [TestMethod]
        public void GetSuiteFailedTestsNightly()
        {
            var testPlanId = 199475;
            var testsIdsUI = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 265004);
            var testsIdsNP = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 264947);
            var testsIdsBlock = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 264926);
            var testsIdsAnalytics = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 279185);
            var testsIdsCdw = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 277588);
            var testsIdsNewRa = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 274457);
            var testsIdsAxe = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 278993);
            var result = new StringBuilder();
            result.AppendLine("UI: " + string.Join(",", testsIdsUI));
            result.AppendLine("Nonparallel: " + string.Join(",", testsIdsNP));
            result.AppendLine("Blocked:" + string.Join(",", testsIdsBlock));
            result.AppendLine("Analytics: " + string.Join(",", testsIdsAnalytics));
            result.AppendLine("NGA CoreDW: " + string.Join(",", testsIdsCdw));
            result.AppendLine("NGA NewRa: " + string.Join(",", testsIdsNewRa));
            result.AppendLine("Axe: " + string.Join(",", testsIdsAxe));

            Assert.Inconclusive(Environment.NewLine + result.ToString());
        }

        [TestMethod]
        public void GetSuiteFailedTestsMajorRelease()
        {
            var testPlanId = 199475;
            var testsIdsFullUs = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 271702);
            var testsIdsFullUk = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 271703);
            var testsIdsNpUs = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 271704);
            var testsIdsNpUk = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 271705);
            var result = new StringBuilder();
            result.AppendLine("UI US: " + string.Join(",", testsIdsFullUs));
            result.AppendLine("UI UK: " + string.Join(",", testsIdsFullUk));
            result.AppendLine("Nonparallel US: " + string.Join(",", testsIdsNpUs));
            result.AppendLine("Nonparallel UK: " + string.Join(",", testsIdsNpUk));

            Assert.Inconclusive(Environment.NewLine + result.ToString());
        }

        [TestMethod]
        public void GetSuiteFailedTestsMinorRelease()
        {
            var testPlanId = 199475;
            var testsIdsFullUs = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 266785);
            var testsIdsFullUk = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 266783);
            var testsIdsNpUs = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 266787);
            var testsIdsNpUk = apiSteps.GetSuiteNotPassedTestIds(testPlanId, 266788);
            var result = new StringBuilder();
            result.AppendLine("UI US: " + string.Join(",", testsIdsFullUs));
            result.AppendLine("UI UK: " + string.Join(",", testsIdsFullUk));
            result.AppendLine("Nonparallel US: " + string.Join(",", testsIdsNpUs));
            result.AppendLine("Nonparallel UK: " + string.Join(",", testsIdsNpUk));

            Assert.Inconclusive(Environment.NewLine + result.ToString());
        }

        [TestMethod]
        public void GetSuiteFailedTestResults()
        {
            var runId = 3428025; // Go to pipeline stage. Check id in the end of the log for 'UI Test Run' task. E.g. 'Test run id: 3424019'
            var testPlanId = 199475;
            var suiteId = 281104;
            var suitetestsIds = apiSteps.GetSuiteNotPassedTestIds(testPlanId, suiteId);

            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            var adoSettings = new AdoSettings(adoSettingsReader);
            var apiStepsNew = new TestRunApiStepsNew(adoSettings);
            var allTestResults = apiStepsNew.GetTrxAttachmentsByRunId(runId);

            var reportBuilder = new RunNewReportBuilder(testSettings.SaveFolder, $"Results_TestPlan{testPlanId}TesSuite{suiteId}RunId{runId}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}");
            var uiReportBuilder = new RunNewUiSummaryBuilder(reportBuilder.Book);
            uiReportBuilder.CreateFullFailedUiReport(ResultReportConverter.ConvertAxe(allTestResults.ToList(), new WorkItemApiSteps(adoSettings)));
            reportBuilder.SaveReport();
        }
    }
}
