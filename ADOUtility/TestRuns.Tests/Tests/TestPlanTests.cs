using ADOCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestRuns.Steps;

namespace TestRuns.Tests.Tests
{
    [TestClass]
    public class TestPlanTests
    {
        private AdoSettings adoSettings;
        private TestPlanApiSteps apiSteps;

        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);
            apiSteps = new TestPlanApiSteps(adoSettings);
        }

        [TestMethod]
        public void GetSuitFailedTests()
        {
            var testPlanId = 199475;
            var suiteId = 271703;
            var testsIds = apiSteps.GetSuiteFailedTestPoint(testPlanId, suiteId);

            Assert.Inconclusive(string.Join(",", testsIds));
        }
    }
}
