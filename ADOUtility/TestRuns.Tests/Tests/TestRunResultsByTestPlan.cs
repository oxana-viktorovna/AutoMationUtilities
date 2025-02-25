﻿using ADOCore;
using ADOCore.Steps;
using ADOCore.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.BuildInTypesExtentions;
using SharedCore.FileUtilities;
using SharedCore.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestRuns.Tests
{
    [TestClass]
    public class TestRunResultsByTestPlan
    {
        private AdoSettings adoSettings;
        private TestPlanApiSteps apiSteps;
        private TestRunTestSettings testSettings;

        private const int TestPlanId = 199475;
        private Dictionary<int, string> NightlySuits = new ()
        {
            { 265004, "UI" },
            { 264947, "NonParallel" },
            { 279185, "Analytics" },
            { 278993, "Axe" },
            { 277588, "CDW" },
            { 274457, "NewRa" }
        };

        private Dictionary<int, string> MajorSuits = new()
        {
            { 280523, "US Analytics" },
            { 271702, "US Full" },
            { 271703, "UK Full" },
            { 271704, "US NonParallel" },
            { 271705, "UK NonParallel" },
            { 278584, "RMI" }
        };

        private Dictionary<int, string> MinorSuits = new()
        {
            { 280538, "US Analytics" },
            { 266785, "US Full" },
            { 266783, "UK Full" },
            { 266787, "US NonParallel" },
            { 266788, "UK NonParallel" }
        };

        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);
            apiSteps = new TestPlanApiSteps(adoSettings);
            var testSettingsReader = new SettingsReader("TestRunTestConfig.json");
            testSettings = new TestRunTestSettings(testSettingsReader);
        }

        [TestMethod]
        public void GetSuiteFailedTestIds()
        {
            var suiteId = 274484;
            var testsIds = apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suiteId);

            Assert.Inconclusive(string.Join(",", testsIds));
        }

        [TestMethod]
        public void GetLastNightlyRunDuration()
        {
            var ids_durations = new Dictionary<int, double>();
            foreach (var suite in NightlySuits)
            {
                var ids_durations_suite = apiSteps.GetSuitePassedTestsDuration(TestPlanId, suite.Key);
                ids_durations.ConcatenateWith2(ids_durations_suite);
            }

            var fileName = "NightlyRunDuration.csv";
            var filePath = Path.Combine(testSettings.SaveFolder, fileName);
            var csv = new CsvWorker(filePath);
            var previous_results = csv.Read();
            var content = new StringBuilder();
            var currentDate = DateTime.Now.ToString("dd-MM-yy");

            if (previous_results == null)
            {
                content.AppendLine($"Test Id,{currentDate}");
                foreach (var idDurationPair in ids_durations)
                    content.AppendLine($"{idDurationPair.Key},{idDurationPair.Value}");

            }
            else
            {
                content.AppendLine($"{string.Join(',',previous_results[0])},{currentDate}");

                for (int i = 1; i < previous_results.Count; i++)
                {
                    var elementToAdd = ",";
                    var id = Convert.ToInt32(previous_results[i][0]);
                    var idDurationPair = ids_durations.FirstOrDefault(idd => idd.Key == id);
                    if (!idDurationPair.Equals(default(KeyValuePair<int, int>)))
                    {
                        elementToAdd = idDurationPair.Value.ToString();
                        ids_durations.Remove(idDurationPair.Key);
                    }

                    content.AppendLine($"{string.Join(',',previous_results[i])},{elementToAdd}");
                }

                var numberOfPreviousRuns = previous_results[0].Count() - 1;
                var emptyRuns = string.Concat(Enumerable.Repeat(",", numberOfPreviousRuns));
                foreach (var idDurationPair in ids_durations)
                {
                    content.AppendLine($"{idDurationPair.Key},{emptyRuns}{idDurationPair.Value}");
                }
            }

            csv.Write(content);
        }

        [TestMethod]
        public void GetSuiteFailedTestIdsNightly()
        {
            var result = new StringBuilder();

            foreach (var suite in NightlySuits)
            {
                var testsIds = apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suite.Key);
                result.AppendLine(suite.Value + ": " + string.Join(",", testsIds));
            }

            Assert.Inconclusive(Environment.NewLine + result.ToString());
        }

        [TestMethod]
        public void GetSuiteFailedTestIdsMajorRelease()
        {
            var result = new StringBuilder();
            foreach (var suite in MajorSuits)
            {
                var testsIds = apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suite.Key);
                result.AppendLine(suite.Value + ": " + string.Join(",", testsIds));
            }

            Assert.Inconclusive(Environment.NewLine + result.ToString());
        }

        [TestMethod]
        public void GetSuiteFailedTestIdsMinorRelease()
        {
            var result = new StringBuilder();
            foreach (var suite in MinorSuits)
            {
                var testsIds = apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suite.Key);
                result.AppendLine(suite.Value + ": " + string.Join(",", testsIds));
            }

            Assert.Inconclusive(Environment.NewLine + result.ToString());
        }

        [TestMethod]
        public void GetSuiteFailedTestResults()
        {
            var runId = 3603820; // Go to pipeline stage. Check id in the end of the log for 'UI Test Run' task. E.g. 'Test run id: 3424019'
            var suiteId = 293755;
            var workItemsApiSteps = new WorkItemApiSteps(adoSettings);
            var fileName = $"Failed_UI_suiteId{suiteId}_runId{runId}";
            var filePath = Path.Combine(testSettings.SaveFolder, fileName + ".csv");
            var csv = new CsvWorker(filePath);

            var suitetestsIds = this.apiSteps.GetSuiteNotPassedTestIds(TestPlanId, suiteId);

            var apiSteps = new TestRunApiSteps(adoSettings);
            var allTestResults = apiSteps.GetTrxAttachmentsByRunId(runId);

            var content = ResultReportConverter.ToCsvContent(allTestResults, workItemsApiSteps);
            csv.Write(content);
        }
    }
}
