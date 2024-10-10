using ADOCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TestRuns.Steps;

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
                "Tracker.Testing.Automation.Tests.Tests.Accessibility.HomePage_AxeTests.T278907_Axe_HomePage",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.MasterFirm_AxeTests.T278844_Axe_MasterFirmInfo",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.MasterFirm_AxeTests.T278932_Axe_MasterFirmOfficeInfo",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.MasterFirm_AxeTests.T278833_Axe_MasterFirmEdit",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.MasterFirm_AxeTests.T278943_Axe_MasterFirmOfficeEdit",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.MasterFirm_AxeTests.T279953_Axe_MasterFirmAuditHistory",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Timekeepers.OCEEvaluation_AxeTests.T279280_Axe_FirmEvaluation_Request",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Timekeepers.OCESetup_AxeTests.T279028_Axe_OCESetup_Index",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Timekeepers.OCESetup_AxeTests.T279036_Axe_OCESetup_Preview",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Timekeepers.OCESetup_AxeTests.T279037_Axe_OCESetup_Edit",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Timekeepers.TkprRatesheet_AxeTests.T278838_Axe_TkprRatesheet",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Search.QuickSearch_AxeTests.T279805_Axe_QuickSearchPage",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Reports.Reports_AxeTests.T278974_Axe_ReportListPage",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Reports.Reports_AxeTests.T278977_Axe_ReportBuilderPage",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Reports.Reports_AxeTests.T278978_Axe_ReportEnginesPage",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Matters.MatterList_AxeTests.T278866_Axe_MatterListingPage",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Matters.Matter_AxeTests.T279563_Axe_MatterInfoPage",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Matters.Matter_AxeTests.T279716_Axe_MatterOptionsPage",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Invoices.Invoices_AxeTests.T278846_Axe_FirmInvoicesPage",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Email.Email_AxeTests.T279091_Axe_CustomEmailSetup",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Email.Email_AxeTests.T279186_Axe_CustomEmailSetupEdit",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Documents.Document_AxeTests.T278996_Axe_MatterDocumentsPage",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Dashboards.MyDashboard_AxeTests.T278902_Axe_MyDashboard",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Budgeting.Budgeting_AxeTests.T278886_Axe_BudgetEdit",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Budgeting.Budgeting_AxeTests.T279556_Axe_BudgetInfo",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Authentication.Logon_AxeTests.T278561_Axe_LogonPage",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Authentication.Logon_AxeTests.T279235_Axe_CompanyLogonPage",
"Tracker.Testing.Automation.Tests.Tests.Accessibility.Accruals.Accruals_AxeTests.T279787_Axe_AccrualsPost",
 };
            var errors = new StringBuilder();
            foreach (var fullTestName in fullTestNames)
            {
                var testNumber = GetTestNumber(fullTestName);
                if (testNumber != 0)
                {
                    var responce = apiSteps.UpdateAutomationAssociation(testNumber, fullTestName);
                    if(responce.StatusCode != HttpStatusCode.OK)
                            errors.AppendLine($"{testNumber} had not been assosiated. {responce.Content}");
                }
            }

            var result = errors.ToString();
            Assert.AreEqual(string.Empty, result, result);
        }

        [TestMethod]
        public void AddLinks()
        {
            var workitem = 276276;
            var linkids = new int[] {
253762,
140648,
140643,
140660,
140656,
253768,
253770,
253769,
140640
 };
            var responce = apiSteps.AddTestedByLinksToWorkItem(workitem, linkids);
            Assert.AreEqual (HttpStatusCode.OK, responce.StatusCode);
        }

        private List<string> GetFullTestNames()
        {
            var filePath = "C:\\Users\\Aksana_Murashka\\Documents\\TRI-SRTR\\BVT\\FullTestNames.txt";
            var fileData = File.ReadAllLines(filePath);
            var regex = new Regex("\\(([^()]*)\\)");
            var result = fileData.Select(d =>
            {
                d = d.Trim().Replace(":", ".");
                var m = regex.Match(d);
                if(!string.IsNullOrEmpty(m.Value))
                    d = d.Replace(m.Value, "");

                return d;
                }).Distinct().ToList();

            return result;
        }

        private List<int> GetTestNumbers(List<string> testNames)
        {
            var result = testNames.Select(name => GetTestNumber(name)).Distinct().ToList();
            result.Remove(0);

            return result;
        }

        private int GetTestNumber(string testName)
        {
            var regex = new Regex(@"T\d+_");
            var number = regex.Match(testName).Groups[0].Value.Replace("T", "").Replace("_", "");

            return string.IsNullOrEmpty(number) ? 0 : Convert.ToInt32(number);
        }
    }
}
