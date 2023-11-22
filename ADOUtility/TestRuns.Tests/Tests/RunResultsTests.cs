using ADOCore;
using ADOCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPOI.SS.Formula.Functions;
using SharedCore.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TestRuns.Models;
using TestRuns.Steps;
using TestRuns.Utilities;

namespace TestRuns
{
    [TestClass]
    public class RunResultsTests
    {
        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);

            var testSettingsReader = new SettingsReader("TestRunTestConfig.json");
            testSettings = new TestRunTestSettings(testSettingsReader);
            apiSteps = new TestRunApiSteps(adoSettings);
            apiStepsNew = new TestRunApiStepsNew(adoSettings);
            buildApiSteps = new BuildApiSteps(adoSettings);
            fileSteps = new ReportFileSteps();

            blockedPattern = "[b|B]locked";
        }

        private AdoSettings adoSettings;
        private TestRunTestSettings testSettings;
        private TestRunApiSteps apiSteps;
        private TestRunApiStepsNew apiStepsNew;
        private BuildApiSteps buildApiSteps;
        private ReportFileSteps fileSteps;
        private string blockedPattern;

        [TestMethod]
        public void GetAllUiTestResultsByBuildId()
        {
            var shortBuildName = buildApiSteps.GetBuildName(testSettings.CurrBuildIds);
            var currFileName = $"All_UI_{shortBuildName}{testSettings.CurrRunPostffix}";
            var reportBuilder = new RunNewReportBuilder(testSettings.SaveFolder, currFileName);
            reportBuilder.DfltFileName = "AllUI";
            var uiReportBuilder = new RunNewUiSummaryBuilder(reportBuilder.Book);

            var allTestResults = apiStepsNew.GetTrxAttachments(testSettings.CurrBuildIds, testSettings.Reruns);
            var uiPassedTests = allTestResults.GetPassedResults();
            var uiFailedTests = allTestResults.GetNotPassedResults();

            var allSelectedTestResults = new List<TestRunUnitTestResult>();
            allSelectedTestResults.AddRange(uiPassedTests);
            allSelectedTestResults.AddRange(uiFailedTests);

            uiReportBuilder.CreateFullFailedUiReport(ResultReportConverter.Convert(allSelectedTestResults));

            reportBuilder.SaveReport();
        }

        [TestMethod]
        public void GetPassedOnReRunUiRunResultsByBuild()
        {
            var shortBuildName = buildApiSteps.GetBuildName(testSettings.CurrBuildIds);
            var currFileName = $"{shortBuildName}{testSettings.CurrRunPostffix}";

            var reportBuilder = new RunNewReportBuilder(testSettings.SaveFolder, currFileName);
            reportBuilder.DfltFileName = "PassedOnrerun";
            var uiReportBuilder = new RunNewUiSummaryBuilder(reportBuilder.Book);

            var allTestResults = apiStepsNew.GetTrxAttachments(testSettings.CurrBuildIds, testSettings.Reruns);
            var uiPassedOnReRunTests = allTestResults.GetPassedOnReRunResults();

            uiReportBuilder.CreateFullFailedUiReport(ResultReportConverter.Convert(uiPassedOnReRunTests));

            reportBuilder.SaveReport();
        }

        [TestMethod]
        public void GetFailedUiRunResultsByBuild()
        {
            var shortBuildName = buildApiSteps.GetBuildName(testSettings.CurrBuildIds);
            var currFileName = $"{shortBuildName}{testSettings.CurrRunPostffix}";

            var reportBuilder = new RunNewReportBuilder(testSettings.SaveFolder, currFileName);
            reportBuilder.DfltFileName = "Failed_UI_";
            var uiReportBuilder = new RunNewUiSummaryBuilder(reportBuilder.Book);

            var uiFailedTests = apiStepsNew.GetTrxAttachments(testSettings.CurrBuildIds, testSettings.Reruns)
                .GetNotPassedResults();

            var uiFailedTestsWithComments = apiStepsNew.CopyCommentsForBlocked(uiFailedTests, testSettings.SaveFolder);

            uiReportBuilder.CreateFullFailedUiReport(uiFailedTestsWithComments);

            reportBuilder.SaveReport();
        }

        [TestMethod]
        public void GetFailedApiRunResults()
        {
            var testResults = apiSteps.GetJUnitAttachements(testSettings.CurrBuildIds);
            var failedResults = testResults.GetFailedTests();

            var shortBuildName = buildApiSteps.GetShortBuildName(testSettings.CurrBuildIds);
            fileSteps.SaveApiFailedResults(testSettings.SaveFolder, "Failed_API_" + shortBuildName, failedResults);
        }

        [TestMethod]
        public void GetReRunString()
        {
            var allTestResults = apiStepsNew.GetTrxAttachments(testSettings.CurrBuildIds, testSettings.Reruns);
            var uiFailedTests = allTestResults.GetNotPassedResults().ExcludeBlocked();

            var groupedByEnv = uiFailedTests.GroupBy(r => r.Env);

            var rerunStrings = new List<string>();
            foreach (var envGroup in groupedByEnv)
            {
                var rerunString = new StringBuilder();
                rerunString.Append("&(");
                foreach (var test in envGroup)
                {
                    var testName = Regex.Replace(test.testName, @"\((.*?)\)", "");
                    rerunString.Append($"Name~{testName}|");
                }
                rerunString.Remove(rerunString.Length - 1, 1); // Remove last | symbol
                rerunString.Append(')');


                rerunStrings.Add($"{envGroup.Key} {envGroup.Count()}{Environment.NewLine}{rerunString}");
            }

            Assert.Inconclusive(string.Join(Environment.NewLine, rerunStrings));
        }

        [TestMethod] //TODO Update
        public void GetUiRunDurationCompare()
        {
            var currAllTestResults = apiSteps.GetAllTrxRunResults(testSettings.CurrBuildId);
            var currUiPassedTests = currAllTestResults.GetPassedResults();
            var currBuildNum = buildApiSteps.GetBuildNumber(testSettings.CurrBuildId);

            var preAllTestResults = apiSteps.GetAllTrxRunResults(testSettings.PreviousBuildId);
            var preUiPassedTests = preAllTestResults.GetPassedResults();
            var preUiTestNames = preUiPassedTests.Select(t => t.testName);
            var preBuildNum = buildApiSteps.GetBuildNumber(testSettings.PreviousBuildId);

            List<(TestRunUnitTestResult currResult, DateTime preDuration)> dureactionCompare = new();
            foreach (var currUiPassedTest in currUiPassedTests)
            {
                if (preUiTestNames.Contains(currUiPassedTest.testName))
                {
                    var preUiPassedTest = preUiPassedTests.FirstOrDefault(t => t.testName == currUiPassedTest.testName);
                    dureactionCompare.Add((currUiPassedTest, preUiPassedTest.duration));
                }
            }

            var currFileName = $"{currBuildNum}{testSettings.CurrRunPostffix}_DurationCompare";
            fileSteps.SaveUiPassedResultsWithDurationComapre(testSettings.SaveFolder, currFileName, dureactionCompare, preBuildNum);
        }

        [TestMethod]
        public void GetUiRunDuration()
        {
            var uiPassedTests = apiStepsNew.GetTrxAttachments(testSettings.CurrBuildIds, testSettings.Reruns).GetPassedResults();

            var shortBuildName = buildApiSteps.GetShortBuildName(testSettings.CurrBuildIds);
            var currFileName = $"{shortBuildName}{testSettings.CurrRunPostffix}_Duration";

            fileSteps.SaveUiPassedResultsWithDuration(testSettings.SaveFolder, currFileName, uiPassedTests);
        }


        [TestMethod]
        public void Interview()
        {
            /*
            var polindrom = IsPolindrom("madam");
            var reverssWords = ReverseWordsOrder("string some other thing");
            var charcount = GetCharCount("madam");
            var noDublicates = RemoveDuplicateChars("madame");
            var allSubstrings = GetAllSubStrings("October");
            var lefRotation = LeftRotation(new int[] { 1, 2, 3, 4, 5 });
            var rightRotation = RightRotation(new int[] { 1, 2, 3, 4, 5 });
            var digets = SplitDigets(1238);
            var max2order = GetSecondMaxNumOrder(new int[] { 1, 2, 8, 4, 5 });
            var max2 = GetSecondMaxNum(new int[] { 1, 2, 8, 4, 5 });
            var n = NoBoringZeros(200200);*/
            var k = dirReduc(new string[] { "NORTH", "SOUTH", "SOUTH", "EAST", "WEST", "NORTH", "WEST" });

        }

        public string[] dirReduc(String[] arr)
        {
            var merge = string.Join(",", arr);
            bool hadBeenRemoved;
            do
            {
                if (merge.Contains("NORTH,SOUTH") || merge.Contains("SOUTH,NORTH")
                  || merge.Contains("EAST,WEST") || merge.Contains("WEST,EAST"))
                {
                    merge = merge.Replace("NORTH,SOUTH", "")
                      .Replace("SOUTH,NORTH", "")
                      .Replace("EAST,WEST", "")
                      .Replace("WEST,EAST", "")
                      .Trim(',');
                    merge = Regex.Replace(merge, @"(,+)", ",");
                    hadBeenRemoved = true;
                }
                else
                    hadBeenRemoved = false;
            }
            while (hadBeenRemoved);

            return merge.Split(",").Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }

        public bool IsPolindrom(string str)
        {
            if (str.Length % 2 == 0)
                return false;

            for (int i = 0, j = str.Length - 1; i < str.Length / 2; i++, j--)
            {
                if (str[i] != str[j])
                    return false;
            }

            return true;
        }

        public string ReverseWordsOrder(string sentence)
            => string.Join(" ", sentence.Split(' ').Reverse());

        public Dictionary<char, int> GetCharCount(string str)
        {
            var result = new Dictionary<char, int>();
            foreach (var ch in str)
            {
                var isExist = result.TryGetValue(ch, out var count);
                if (isExist)
                    result[ch] = count + 1;
                else
                    result.Add(ch, 1);
            }

            return result;
        }

        public string RemoveDuplicateChars(string str)
        {
            var chars = str.ToCharArray();
            var result = string.Empty;
            foreach (var ch in chars)
            {
                if (!result.Contains(ch))
                    result += ch;
            }

            return result;
        }

        public IEnumerable<string> GetAllSubStrings(string str)
        {
            var result = new List<string>();
            for (int i = 0; i < str.Length; ++i)
            {
                StringBuilder subString = new StringBuilder(str.Length - i);
                for (int j = i; j < str.Length; ++j)
                {
                    subString.Append(str[j]);
                    result.Add(subString.ToString());
                }
            }

            return result;
        }

        public int[] LeftRotation(int[] arr)
        {
            var firstElement = arr[0];
            for (int i = 1; i < arr.Length; ++i)
            {
                arr[i - 1] = arr[i];
            }
            arr[arr.Length - 1] = firstElement;

            return arr;
        }

        public int[] RightRotation(int[] arr)
        {
            var lastElement = arr[arr.Length - 1];
            for (int i = arr.Length - 1; i > 0; --i)
            {
                arr[i] = arr[i - 1];
            }
            arr[0] = lastElement;

            return arr;
        }

        public IEnumerable<int> SplitDigets(int num)
        {
            var result = new List<int>();
            while (num > 0)
            {
                result.Add(num % 10);
                num /= 10;
            }

            return result;
        }

        public int GetSecondMaxNumOrder(int[] arr)
        {
            var orderedArr = arr.OrderByDescending(x => x).ToArray();
            return orderedArr[1];
        }

        public int GetSecondMaxNum(int[] arr)
        {
            var max1 = int.MinValue;
            var max2 = int.MinValue;
            foreach (var num in arr)
            {
                if (num > max1)
                {
                    max2 = max1;
                    max1 = num;
                }
                else if (num > max2 && num != max1)
                    max2 = num;
            }

            return max2;
        }

        public static int NoBoringZeros(int n)
        {
            var result = new List<int>();
            while (n % 10 == 0)
            {
                n /= 10;
            }

            return n;
        }
    }
}
