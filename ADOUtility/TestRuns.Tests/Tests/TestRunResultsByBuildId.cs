using ADOCore;
using ADOCore.Models;
using ADOCore.Models.WiqlQuery;
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
using System.Text.RegularExpressions;

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

            if(BuildId == -1)
                Assert.Inconclusive("Populate BuildId at the class. Id can be taken from pipeline's Url");
        }

        private AdoSettings adoSettings;
        private TestRunTestSettings testSettings;
        private TestRunApiSteps apiStepsNew;
        private BuildApiSteps buildApiSteps;
        private WorkItemApiSteps workItemApiSteps;
        private const int BuildId = 697498;

        [TestMethod]
        public void GetDiffAxeResults_Csv()
        {
            var previousBuildIds = new List<int> { 697201, 697384 }; // first original run, then rerun
            var currentBuildIds = new List<int> { 697201, 697639 };
            var currentDate = DateTime.Now.ToString("dd-MM-yy-HH-mm-ss");
            var fileName = $"Axe_Diff_{currentDate}";
            var filePath = Path.Combine(testSettings.SaveFolder, fileName + ".csv");
            var csv = new CsvWorker(filePath);
            var content = new StringBuilder($"Test Number,Test Name,Previous,Current,AreaPath,LinkedBugs,CriticalDiff,SeriousDiff{Environment.NewLine}");
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

                    var startIndex = error.IndexOf("Report can be found");
                    if (startIndex != -1)
                        error = error.Substring(0, startIndex);

                    var pattern = @"(?i)Id=(\d+|""\d+"")";
                    var replacement = "Id=XXXX";
                    error = Regex.Replace(error, pattern, replacement, RegexOptions.IgnoreCase);

                    pattern = @"^[{(]?[0-9a-fA-F]{8}[-]?[0-9a-fA-F]{4}[-]?[0-9a-fA-F]{4}[-]?[0-9a-fA-F]{4}[-]?[0-9a-fA-F]{12}[)}]?$ ";
                    error = Regex.Replace(error, pattern, match => new string('x', match.Value.Length));

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
            var itemsToRemove = new List<TestRunUnitTestResult>();
            foreach (var id in onlyInCurrent)
            {
                var itemToRemove = currentTrx.FirstOrDefault(i => i.TestNumber == id);
                if (itemToRemove == null)
                    continue;

                itemsToRemove.Add(itemToRemove);

                if (itemToRemove.outcome != "Passed")
                {
                    var test_Item = workItemApiSteps.GetWorkItem(itemToRemove.TestNumber);
                    var areaPath = test_Item.fields.AreaPath;
                    var bugs_items = GetBugsLinkedToTest(itemToRemove.TestNumber);
                    var bugs = string.Join(';', bugs_items.Select(b => b.target.id));
                    var error = getErrorMessage(itemToRemove.Output);
                    var seriousNum = GetNumberOfViolations(error, "serious");
                    var criticalNum = GetNumberOfViolations(error, "critical");
                    content.AppendLine($"{id},{itemToRemove.testName},,{error},{areaPath},{bugs},{criticalNum},{seriousNum}");
                }
            }
            foreach (var itemToRemove in itemsToRemove)
                currentTrx.Remove(itemToRemove);

            var onlyInPrevious = previousIds.Except(currentIds);
            itemsToRemove = new List<TestRunUnitTestResult>();
            foreach (var id in onlyInPrevious)
            {
                var itemToRemove = previousTrx.FirstOrDefault(i => i.TestNumber == id);
                if (itemToRemove == null)
                    continue;

                itemsToRemove.Add(itemToRemove);

                if (itemToRemove.outcome != "Passed")
                {
                    var test_Item = workItemApiSteps.GetWorkItem(itemToRemove.TestNumber);
                    var areaPath = test_Item.fields.AreaPath;
                    var bugs_items = GetBugsLinkedToTest(itemToRemove.TestNumber);
                    var bugs = string.Join(';', bugs_items.Select(b => b.target.id));
                    var error = getErrorMessage(itemToRemove.Output);
                    var seriousNum = GetNumberOfViolations(error, "serious");
                    var criticalNum = GetNumberOfViolations(error, "critical");
                    content.AppendLine($"{id},{itemToRemove.testName},{error},,{areaPath},{bugs},-{criticalNum},-{seriousNum}");
                }
            }
            foreach (var itemToRemove in itemsToRemove)
                previousTrx.Remove(itemToRemove);

            foreach (var curTrx in currentTrx)
            {
                var prevTrx = previousTrx.FirstOrDefault(i => i.TestNumber == curTrx.TestNumber);
                if (curTrx.outcome == "Passed" && prevTrx.outcome == "Passed")
                    continue;

                var prevError = getErrorMessage(prevTrx.Output);
                var prevSeriousNum = GetNumberOfViolations(prevError, "serious");
                var prevCriticalNum = GetNumberOfViolations(prevError, "critical");

                var curError = getErrorMessage(curTrx.Output);
                var curSeriousNum = GetNumberOfViolations(curError, "serious");
                var curCriticalNum = GetNumberOfViolations(curError, "critical");

                var seriousDiff = curSeriousNum - prevSeriousNum;
                var criticalDiff = curCriticalNum - prevCriticalNum;

                var areaPath = workItemApiSteps.GetWorkItem(Convert.ToInt32(curTrx.TestNumber)).fields.AreaPath;
                var bugs = GetBugsLinkedToTest(curTrx.TestNumber);
                var bugsStr = string.Join(';', bugs.Select(b => b.target.id));

                var error_content = "";

                if (curTrx.outcome != "Passed" && prevTrx.outcome == "Passed")
                    error_content = $"Passed,{curError}";
                else if (curTrx.outcome == "Passed" && prevTrx.outcome != "Passed")
                    error_content = $"{prevError},Passed";
                else if (curError != prevError)
                    error_content = $"{prevError},{curError}";
                else if(curError.Contains(nonAxePrefix))
                    error_content = $"same-->,{curError}";

                if(!string.IsNullOrEmpty(error_content))
                    content.AppendLine($"{curTrx.TestNumber},{curTrx.testName},{error_content},{areaPath},{bugsStr},{criticalDiff},{seriousDiff}");
            }

            csv.Write(content);
        }

        private int? GetNumberOfViolations(string error, string type)
        {
            if (string.IsNullOrEmpty(error))
                return 0;

            var pattern = @$"{type}:\s*(\d+)";
            Match match = Regex.Match(error, pattern);

            return match.Success && int.TryParse(match.Groups[1].Value, out int number) ? number : 0;
        }

        private Workitemrelation[] GetBugsLinkedToTest(int? testNumber)
        {
            if (testNumber == null)
                return null;

            var wiqlSteps = new WiqlQuerySteps(adoSettings);
            var query = new WiqlDirectLinksQueryBuilder().AddAttributesToGet("[System.Id]")
                .AddSourceCondition(null, "[System.Id]", WiqlConsnt.Operator.Equal, testNumber)
                .AddTargetCondition(null, WorkItemFields.GetAdoName("Type"), WiqlConsnt.Operator.Equal, "Bug")
                .AddTargetCondition(WiqlConsnt.Conjunction.AndNot, WorkItemFields.GetAdoName("State"), WiqlConsnt.Operator.In, "('Removed', 'Closed')")
                .Build();
            var bugs = wiqlSteps.GetLinkedItems(query);

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

            var contentUI = ResultReportConverter.ToCsvContent(notPassedUIResults, workItemApiSteps);
            csv.Write(contentUI);
        }

        [TestMethod]
        public void GetAxeTestResultsByBuildId_NotPassed_Csv()
        {
            var shortBuildName = buildApiSteps.GetFullBuildName(BuildId).Replace(".", "_");
            var fileName = $"Failed_AXE_{shortBuildName}";
            var filePath = Path.Combine(testSettings.SaveFolder, fileName + ".csv");
            var csv = new CsvWorker(filePath);

            var allTestResults = apiStepsNew.GetTrxAttachments(BuildId, "Axe");
            var notPassedAxeResults = allTestResults.GetAxeNotPassedResults();

            var contentAxe = ResultReportConverter.ToCsvContent(notPassedAxeResults, workItemApiSteps);
            csv.Write(contentAxe);

            var nonAxeFailures = notPassedAxeResults.Where(r => r.outcome == "NON-A11y FAILURE").Select(r => r.TestNumber);
            Assert.Inconclusive(Environment.NewLine + "NonAxe failures: " + string.Join(",", nonAxeFailures));
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
