using ADOCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using TestRuns.Steps;
using TestRuns.Utilities;

namespace TestRuns
{
    [TestClass]
    public class PlanResultTests
    {
        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);

            var testSettingsReader = new SettingsReader("TestRunTestConfig.json");
            testSettings = new TestRunTestSettings(testSettingsReader);
            apiStepsNew = new TestRunApiStepsNew(adoSettings);
        }

        private AdoSettings adoSettings;
        private TestRunTestSettings testSettings;
        private TestRunApiStepsNew apiStepsNew;

        [TestMethod]
        public void GetTestIdsOfTestSuitInTestPlan()
        {
            var currFileName = $"Test_Cases_Ids_And_Names_From_Test_Plan_{testSettings.TestPlanId}";
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
