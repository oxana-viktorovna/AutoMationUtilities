using ADOCore;
using ADOCore.Steps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace TestRuns.Tests
{
    [TestClass]
    public class WorkItemsTests
    {
        private AdoSettings adoSettings;
        private WorkItemApiSteps apiSteps;

        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);
            apiSteps = new WorkItemApiSteps(adoSettings);
        }

        [TestMethod]
        public void UpdateAutoAssociation()
        {
            var fullTestNames = new List<string>()
            {
                "Tracker.Testing.Automation.Tests.Tests.Accessibility.SystemSetup.Security_AxeTests.T284128_Axe_FirmUserMatterAccess",
                "Tracker.Testing.Automation.Tests.Tests.Accessibility.SystemSetup.SystemAdministration_AxeTests.T284127_Axe_TestActionRetryRedirect",
                "Tracker.Testing.Automation.Tests.Tests.Accessibility.SystemSetup.MattersSetup.MattersSetup_AxeTests.T284125_Axe_MatterCategoryTree",
                "Tracker.Testing.Automation.Tests.Tests.Accessibility.SystemSetup.MattersSetup.MattersSetup_AxeTests.T284126_Axe_SubstantiveLawTree"
 };
            var errors = new StringBuilder();
            foreach (var fullTestName in fullTestNames)
            {
                var testNumber = GetTestNumber(fullTestName);
                if (testNumber != 0)
                {
                    var responce = apiSteps.UpdateAutomationAssociation(testNumber, fullTestName);
                    if (responce.StatusCode != HttpStatusCode.OK)
                        errors.AppendLine($"{testNumber} had not been assosiated. {responce.Content}");
                }
            }

            var result = errors.ToString();
            Assert.AreEqual(string.Empty, result, result);
        }

        [TestMethod]
        public void AddRelatedLinks()
        {
            var workitem = 284895;
            var linkids = new int[] {
                157754,
                180853
        };
            var responce = apiSteps.AddRelatedLinksToWorkItem(workitem, linkids);
            Assert.AreEqual(HttpStatusCode.OK, responce.StatusCode);
        }

        private int GetTestNumber(string testName)
        {
            var regex = new Regex(@"T\d+_");
            var number = regex.Match(testName).Groups[0].Value.Replace("T", "").Replace("_", "");

            return string.IsNullOrEmpty(number) ? 0 : Convert.ToInt32(number);
        }
    }
}
