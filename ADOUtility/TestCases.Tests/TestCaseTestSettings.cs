using SharedCore.Settings;
using System;
using System.Collections.Generic;

namespace TestCases.Tests
{
    public class TestCaseTestSettings
    {
        public TestCaseTestSettings(SettingsReader reader)
        {
            SaveFolder = reader.GetSetting("saveFolder");
            CurrBuildId = GetIntSetting(reader, "currentTestRun:buildId");
            CurrBuildIds = reader.GetSettingArray<int>("currentTestRun:buildIds");
            CurrRunPostffix = GetPostfix(reader, "currentTestRun:runPostffix");
            TestPlanId = GetIntSetting(reader, "currentTestRun:TestPlanId");
            TestSuitId = GetIntSetting(reader, "currentTestRun:TestSuitId");
        }

        public string SaveFolder { get; private set; }
        public int CurrBuildId { get; private set; }
        public List<int> CurrBuildIds { get; private set; }
        public string CurrRunPostffix { get; private set; }
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
