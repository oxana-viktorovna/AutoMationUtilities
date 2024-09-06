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
                "Tracker.Testing.Automation.Tests.Tests.Authentication.CiamSAMLUserLoginTests.T240225_CiamSAML_ActiveUserLogin",
 "Tracker.Testing.Automation.Tests.Tests.Authentication.CiamSAMLUserLoginTests.T263491_CiamSAML_ActiveUserLogin_WrongPassword",
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
