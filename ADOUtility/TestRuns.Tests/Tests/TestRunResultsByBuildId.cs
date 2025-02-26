using ADOCore;
using ADOCore.Models;
using ADOCore.Steps;
using ADOCore.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.FileUtilities;
using SharedCore.Settings;
using SharedCore.StringUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestRuns.Tests
{
    [TestClass]
    public class TestRunResultsByBuildId
    {
        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);

            var testSettingsReader = new SettingsReader("TestRunTestConfig.json");
            testSettings = new TestRunTestSettings(testSettingsReader);
            apiStepsNew = new TestRunApiSteps(adoSettings);
            buildApiSteps = new BuildApiSteps(adoSettings);
            workItemApiSteps = new WorkItemApiSteps(adoSettings);
        }

        private AdoSettings adoSettings;
        private TestRunTestSettings testSettings;
        private TestRunApiSteps apiStepsNew;
        private BuildApiSteps buildApiSteps;
        private WorkItemApiSteps workItemApiSteps;
        private const int BuildId = 696382;

        [TestMethod]
        public void GetDiffAxeResults_Csv()
        {
            var previousBuildIds = new List<int> { 695475, 695623}; // first original run, then rerun
            var currentBuildIds = new List<int> { 695701, 695767 };
            var currentDate = DateTime.Now.ToString("dd-MM-yy-HH-mm-ss");
            var fileName = $"Axe_Diff_{currentDate}";
            var filePath = Path.Combine(testSettings.SaveFolder, fileName + ".csv");
            var csv = new CsvWorker(filePath);
            var content = new StringBuilder($"Test Number,Test Name,Previous,Current,AreaPath,LinkedBugs{Environment.NewLine}");
            var nonAxePrefix = "NON AXE FAILURE";
            Func<TestRunUnitTestResultOutput, string> getErrorMessage = output
                =>
                {
                    var error = output.ErrorInfo == null
                            ? string.Empty
                            : output.ErrorInfo.Message.Replace(",", " ").Replace(Environment.NewLine, " ");
                    if (!error.Contains("Accessibility violations"))
                        error = nonAxePrefix + " --> " + error;

                    error = error.Replace("Multiple failures or warnings in test:", "")
                            .Replace("Accessibility violations found on ","");

                    if (error.Length > 32760)
                        error = error.Substring(0, 32760);

                    return error.Trim();
                };

            var previousTrx = new List<TestRunUnitTestResult>();
            foreach (var buildId in previousBuildIds)
            {
                var buildTrx = apiStepsNew.GetTrxAttachments(buildId, "Axe");
                if (!previousTrx.Any())
                    previousTrx.AddRange(buildTrx);
                else
                {
                    //replace original val with rerun ones
                    foreach (var rerunTrx in buildTrx)
                    {
                        var origRun = previousTrx.FirstOrDefault(i => i.TestNumber == rerunTrx.TestNumber);
                        if (origRun != default)
                            previousTrx.Remove(origRun);

                        previousTrx.Add(rerunTrx);
                    }
                }
            }

            var currentTrx = new List<TestRunUnitTestResult>();
            foreach (var buildId in currentBuildIds)
            {
                var buildTrx = apiStepsNew.GetTrxAttachments(buildId, "Axe");
                if (!currentTrx.Any())
                    currentTrx.AddRange(buildTrx);
                else
                {
                    //replace original val with rerun ones
                    foreach (var rerunTrx in buildTrx)
                    {
                        var origRun = currentTrx.FirstOrDefault(i => i.TestNumber == rerunTrx.TestNumber);
                        if (origRun != default)
                            currentTrx.Remove(origRun);

                        currentTrx.Add(rerunTrx);
                    }
                }
            }

            // compare previous and current
            var currentIds = currentTrx.Select(i => i.TestNumber);
            var previousIds = previousTrx.Select(i => i.TestNumber);
            var onlyInCurrent = currentIds.Except(previousIds);
            foreach (var id in onlyInCurrent)
            {
                var itemToRemove = currentTrx.FirstOrDefault(i => i.TestNumber == id);
                if (itemToRemove != null)
                    currentTrx.Remove(itemToRemove);

                if (itemToRemove.outcome != "Passed")
                {
                    var areaPath = workItemApiSteps.GetWorkItem(Convert.ToInt32(itemToRemove.TestNumber)).fields.AreaPath;
                    var bugs = GetBugsLinkedToTest(itemToRemove.TestNumber);
                    content.AppendLine($"{id},,{getErrorMessage(itemToRemove.Output)},{areaPath},{string.Join(';', bugs.Select(b => b.target.id))}");
                }
            }

            var onlyInPrevious = previousIds.Except(currentIds);
            foreach (var id in onlyInPrevious)
            {
                var itemToRemove = previousTrx.FirstOrDefault(i => i.TestNumber == id);
                if (itemToRemove != null)
                    previousTrx.Remove(itemToRemove);

                if (itemToRemove.outcome != "Passed")
                {
                    var areaPath = workItemApiSteps.GetWorkItem(Convert.ToInt32(itemToRemove.TestNumber)).fields.AreaPath;
                    var bugs = GetBugsLinkedToTest(itemToRemove.TestNumber);
                    content.AppendLine($"{id},{getErrorMessage(itemToRemove.Output)}, NOT INCLUDED TO THE RUN,{areaPath},{string.Join(';', bugs.Select(b => b.target.id))}");
                }
            }

            foreach (var curTrx in currentTrx)
            {
                var prevTrx = previousTrx.FirstOrDefault(i => i.TestNumber == curTrx.TestNumber);
                if (curTrx.outcome == "Passed" && prevTrx.outcome == "Passed")
                    continue;

                var testIdName = $"{curTrx.TestNumber},{curTrx.testName}";
                var prevError = getErrorMessage(prevTrx.Output);
                var curError = getErrorMessage(curTrx.Output);
                var areaPath = workItemApiSteps.GetWorkItem(Convert.ToInt32(curTrx.TestNumber)).fields.AreaPath;
                var bugs = GetBugsLinkedToTest(curTrx.TestNumber);
                var bugsStr = string.Join(';', bugs.Select(b => b.target.id));
                if (curTrx.outcome != "Passed" && prevTrx.outcome == "Passed")
                    content.AppendLine($"{testIdName},Passed,{curError},{areaPath},{bugsStr}");
                else if (curTrx.outcome == "Passed" && prevTrx.outcome != "Passed")
                    content.AppendLine($"{testIdName},{prevError},Passed,{areaPath},{bugsStr}");
                else if (curError != prevError)
                    content.AppendLine($"{testIdName},{prevError},{curError},{areaPath},{bugsStr}");
                else if(curError.Contains(nonAxePrefix))
                    content.AppendLine($"{testIdName},same-->,{curError},{areaPath},{bugsStr}");
            }

            csv.Write(content);
        }

        private Workitemrelation[] GetBugsLinkedToTest(int? testNumber)
        {
            if (testNumber == null)
                return null;

            var wiqlSteps = new WiqlQuerySteps(adoSettings);

            var bugs = wiqlSteps.GetLinkedItems($@"SELECT [System.Id]
            FROM workitemLinks
            WHERE
                (
                    [Source].[System.TeamProject] = @project
                    AND [Source].[System.Id] = {testNumber}
                )
                AND (
                    [Target].[System.TeamProject] = @project
                    AND [Target].[System.WorkItemType] = 'Bug'
                    AND NOT [Target].[System.State] IN ('Removed', 'Closed')
                )
            ORDER BY [System.Id]
            MODE (MustContain)");

            return bugs.GroupBy(b => b.target.id).Select(g => g.FirstOrDefault()).ToArray();
        }

        [TestMethod]
        public void GetUITestResultsByBuildId_NotPassed_Csv()
        {
            var shortBuildName = buildApiSteps.GetFullBuildName(BuildId).Replace(".","_");
            var fileName = $"Failed_UI_{shortBuildName}";
            var filePath = Path.Combine(testSettings.SaveFolder, fileName+".csv");
            var csv = new CsvWorker(filePath);

            var allTestResults = apiStepsNew.GetTrxAttachments(BuildId);
            var notPassedUIResults = allTestResults.GetNotPassedResults();
            var notPassedAxeResults = allTestResults.GetAxeNotPassedResults();

            var contentUI = ResultReportConverter.ToCsvContent(notPassedUIResults, workItemApiSteps);
            var contentAxe = ResultReportConverter.ToCsvContent(notPassedAxeResults, workItemApiSteps);
            var content = contentUI.Concat(contentAxe);
            csv.Write(content);
        }

        [TestMethod]
        public void GetUITestResultsByBuildId_PassedOnReRun_Csv()
        {
            var shortBuildName = buildApiSteps.GetFullBuildName(BuildId).Replace(".", "_");
            var fileName = $"PassedOnRerun_UI_{shortBuildName}";
            var filePath = Path.Combine(testSettings.SaveFolder, fileName + ".csv");
            var csv = new CsvWorker(filePath);

            var allTestResults = apiStepsNew.GetTrxAttachments(BuildId);
            var uiPassedOnReRunTests = allTestResults.GetPassedOnReRunResults();

            var content = ResultReportConverter.ToCsvContent(uiPassedOnReRunTests);
            csv.Write(content);
        }

        [TestMethod]
        public void GetReRunStringByBuildId()
        {
            var allTestResults = apiStepsNew.GetTrxAttachments(BuildId);
            var uiFailedTests = allTestResults.GetNotPassedResults().ExcludeBlocked();

            var testIds = uiFailedTests.Select(test => test.testName.GetTestCaseNumber());

            Assert.Inconclusive("&(Name~" + string.Join("|Name~", testIds) +")");
        }
    }
}
