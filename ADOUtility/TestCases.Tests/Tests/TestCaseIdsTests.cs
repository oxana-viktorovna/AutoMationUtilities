using ADOCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using TestCases.Steps;
using TestCases.Utilities;

namespace TestCases.Tests.Tests
{
    [TestClass]
    public class TestCaseIdsTests
    {
        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);

            var testSettingsReader = new SettingsReader("TestCaseTestConfig.json");
            testSettings = new TestCaseTestSettings(testSettingsReader);
            apiStepsNew = new TestCaseApiStepsNew(adoSettings);
            buildApiSteps = new BuildApiSteps(adoSettings);
        }

        private AdoSettings adoSettings;
        private TestCaseTestSettings testSettings;
        private TestCaseApiStepsNew apiStepsNew;
        private BuildApiSteps buildApiSteps;

        [TestMethod]
        public void GetTestIdsOfTestSuitInTestPlan()
        {
            var shortBuildName = buildApiSteps.GetBuildName(testSettings.CurrBuildIds);
            var currFileName = $"Test_Ids_{shortBuildName}{testSettings.CurrRunPostffix}";
            var reportBuilder = new RunNewReportBuilder(testSettings.SaveFolder, currFileName);
            reportBuilder.DfltFileName = "TestIds";
            var uiReportBuilder = new RunNewUiSummaryBuilder(reportBuilder.Book);

            var testResults = apiStepsNew.GetTestIdNamePairs(testSettings.TestPlanId, testSettings.TestSuitId);
            var testIds = apiStepsNew.DivideIntoBatches(testResults, 3);

            uiReportBuilder.CreateTestIdsReport(ResultReportConverter.ConvertToTestInfo(testIds));
            reportBuilder.SaveReport();
        }
    }
}
