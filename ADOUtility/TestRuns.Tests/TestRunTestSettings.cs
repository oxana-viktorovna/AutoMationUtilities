using SharedCore.Settings;
using System;
using System.Collections.Generic;

namespace TestRuns
{
    public class TestRunTestSettings
    {
        public TestRunTestSettings(SettingsReader reader)
        {
            SaveFolder = reader.GetSetting("saveFolder");
            CurrBuildId = GetIntSetting(reader, "currentTestRun:buildId");
            Reruns = reader.GetSettingArray<int>("currentTestRun:RerunBuildIds");
            CurrBuildIds = reader.GetSettingArray<int>("currentTestRun:buildIds");
            RunDuration = reader.GetSetting("currentTestRun:runDuration");
            PreviousBuildId = GetIntSetting(reader, "previousTestRun:buildId");
            CurrRunPostffix = GetPostfix(reader,"currentTestRun:runPostffix");
            PreviousRunPostffix = GetPostfix(reader,"previousTestRun:runPostffix");
            TestPlanId = GetIntSetting(reader, "testCasesInfo:TestPlanId");
            TestSuitId = GetIntSetting(reader, "testCasesInfo:TestSuitId");
        }

        public string SaveFolder { get; private set; }
        public int CurrBuildId { get; private set; }
        public List<int> Reruns { get; private set; }
        public List<int> CurrBuildIds { get; private set; }
        public string RunDuration { get; private set; }
        public int PreviousBuildId { get; private set; }
        public string CurrRunPostffix { get; private set; }
        public string PreviousRunPostffix { get; private set; }
        public int TestPlanId { get; private set; }
        public int TestSuitId { get; private set; }

        private int GetIntSetting(SettingsReader reader, string settingName)
        {
            var buildIdStr = reader.GetSetting(settingName);

            return string.IsNullOrEmpty(buildIdStr)
                ? 0
                : Convert.ToInt32(buildIdStr);
        }

        private string GetPostfix(SettingsReader reader, string settingName)
        {
            var value = reader.GetSetting(settingName);

            return !string.IsNullOrEmpty(value) ? $"_{value}" : value;
        }
    }
}
