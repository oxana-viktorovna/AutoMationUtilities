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
                "Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListApprovalRoutingTests.T18325_InvoiceReview_AR2_Approve_OnGrid",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListApprovalRoutingTests.T18324_InvoiceReview_AR1_Approve_OnGrid",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListApprovalRoutingTests.T19000_InvoiceReview_AR1_Approve_OnGrid_Undo_ReApprove",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListApprovalRoutingTests.T129825_InvoiceReview_AR3_Approve_OnGrid",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T197826_InvoiceReview_PendingInvoicesTab_AuditStatusFilter",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T243033_InvoiceReview_PendingInvoicesTab_AuditTypeFilter_BudgetsAlertAudits",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T243036_InvoiceReview_PendingInvoicesTab_AuditTypeFilter_TkprAudits",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T243034_InvoiceReview_PendingInvoicesTab_AuditTypeFilter_BillingPeriodAudits",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T243032_InvoiceReview_PendingInvoicesTab_AuditTypeFilter_TaskCodeAlertAudits",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T198383_InvoiceReview_PendingInvoicesTab_NewTkprAudits_RedFlag_Resolved_AlertIcon",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T233338_InvoiceReview_PendingInvoicesTab_TkprRateChangeAudit_RedFlag_AuditsResolved_AlertIcon",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T233341_InvoiceReview_PendingInvoicesTab_AuditsResolved_AlertIcon_BillingPeriodAudit",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T233342_InvoiceReview_PendingInvoicesTab_BudgetAudit_AuditsResolved_AlertIcon",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T243035_InvoiceReview_PendingInvoicesTab_AuditTypeFilter_ExpenseRateViolations_RedFlag",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T233339_InvoiceReview_PendingInvoicesTab_AuditsResolved_AlertIcon_ExpenseRateViolationAudit_RedFlag",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T233340_InvoiceReview_PendingInvoicesTab_AuditsResolved_AlertIcon_ExpenseTotalViolationAudit_RedFlag",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListAuditTests.T233343_InvoiceReview_PendingInvoicesTab_AuditsResolved_AlertIcon_TaskCodeAudit",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListFiltersTests.T205254_InvoiceReview_UserFilter",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListFiltersTests.T197839_InvoiceReview_PendingInvoicesTab_CountryFilter",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListFiltersTests.T198295_InvoiceReview_PendingInvoicesTab_PracticeGroupFilter",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListFiltersTests.T198294_InvoiceReview_PendingInvoicesTab_OrganizationalUnitFilter",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListFiltersTests.T198293_InvoiceReview_PendingInvoicesTab_MatterTypeFilter",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListFiltersTests.T146384_InvoiceReview_ApproveAndGoNext_Filtering",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListQuickFiltersTests.T267769_InvoiceReview_PendingInvoicesTab_QuickFilters_Audits",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListQuickFiltersTests.T267770_InvoiceReview_PendingInvoicesTab_QuickFilters_NoAudits",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListQuickFiltersTests.T267771_InvoiceReview_PendingInvoicesTab_QuickFilters_WithExpenses",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListQuickFiltersTests.T267772_InvoiceReview_PendingInvoicesTab_QuickFilters_WithoutExpenses",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListQuickFiltersTests.T267773_InvoiceReview_PendingInvoicesTab_QuickFilters_Range",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListQuickFiltersTests.T267774_InvoiceReview_PendingInvoicesTab_QuickFilters_30Days",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListQuickFiltersTests.T267775_InvoiceReview_PendingInvoicesTab_QuickFilters_60Days",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListQuickFiltersTests.T267776_InvoiceReview_PendingInvoicesTab_QuickFilters_90Days",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListQuickFiltersTests.T267777_InvoiceReview_PendingInvoicesTab_QuickFilters_AllInvoices",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListQuickFiltersTests.T198353_InvoiceReview_PendingInvoicesTab_QuickFilters",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListQuickFiltersTests.T205386_InvoiceReview_RecentlyReviewedTab_QuickFilters",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListQuickFiltersTests.T200211_InvoiceReview_RecentlyReviewedTab_QuickFilters",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListRecentlyReviewedTests.T205387_InvoiceReview_RecentlyReviewedTab_UnnapproveAction",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListRecentlyReviewedTests.T195366_InvoiceReview_RecentlyReviewedTab_Unapprove_ApprovedLedesInvoices",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListRecentlyReviewedTests.T238896_InvoiceReview_RecentlyReviewedTab_Unapprove_ReducedLedesInvoices",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListRecentlyReviewedTests.T232785_InvoiceReview_RecentlyReviewedTab_Unapprove_NonLedesInvoices",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListRecentlyReviewedTests.T232747_InvoiceReview_RecentlyReviewedTab_Unapprove_LiveInvoices",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListRecentlyReviewedTests.T236555_InvoiceReview_RecentlyReviewedTab_Sorting",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T205385_InvoiceReview_PendingInvoicesTab_AddComments",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T205390_InvoiceReview_PendingInvoicesTab_ApproveSelected",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T240907_InvoiceReview_PendingInvoicesTab_ApproveOneInvoice",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T205428_InvoiceReview_PendingInvoicesTab_ColumnsCustomize",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T241387_InvoiceReview_PendingInvoicesTab_Filters_Separately",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T205427_InvoiceReview_PendingInvoicesTab_Filters_Simultaneously",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T205429_InvoiceReview_PendingInvoicesTab_CustomDrawerSection",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T205241_InvoiceReview_PendingInvoicesTab_ChartsSection",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T205240_InvoiceReview_NavigationOnLandingPageAndInvoiceList",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T205388_InvoiceReview_PendingInvoicesTab_Paginator",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T192075_InvoiceReview_PendingInvoicesTab_PendingInvoiceAmounts_SeeDetailsModalDialog",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T201820_InvoiceReview_PendingInvoicesTab_Sorting",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T198802_InvoiceReview_PendingInvoicesTab_DrawerSection_APCodes",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T198055_InvoiceReview_PendingInvoicesTab_APCodes_InDrawerSection_ByAmount",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T190846_InvoiceReview_PendingInvoicesTab_GridIsDisplayed_WithInvoices",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T190843_InvoiceReview_PendingInvoicesTab_GridIsDisplayed_NoInvoices",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T198088_InvoiceReview_PendingInvoicesTab_PriorAction_DrawerSection",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T118523_InvoiceReview_ApproveAndGoNext",
"Tracker.Testing.Automation.Tests.Tests.InvoiceList.InvoiceListTests.T195959_InvoiceReview_PendingInvoicesTab_ProformaInvoicesInfo_IsDisplayed"
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
