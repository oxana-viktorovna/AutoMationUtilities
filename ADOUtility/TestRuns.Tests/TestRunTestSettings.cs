using SharedCore.Settings;
using System;

namespace TestRuns
{
    public class TestRunTestSettings
    {
        public TestRunTestSettings(SettingsReader reader)
        {
            SaveFolder = reader.GetSetting("saveFolder");
            CurrBuildId = GetIntSetting(reader, "currentTestRun:buildId");
            RunDuration = reader.GetSetting("currentTestRun:runDuration");
            PreviousBuildId = GetIntSetting(reader, "previousTestRun:buildId");
            CurrRunPostffix = GetPostfix(reader,"currentTestRun:runPostffix");
            PreviousRunPostffix = GetPostfix(reader,"previousTestRun:runPostffix");
            BlockedTestRun = GetIntSetting(reader, "currentTestRun:blockedTestRun");
        }

        public string SaveFolder { get; private set; }
        public int CurrBuildId { get; private set; }
        public string RunDuration { get; private set; }
        public int BlockedTestRun { get; private set; }
        public int PreviousBuildId { get; private set; }
        public string CurrRunPostffix { get; private set; }
        public string PreviousRunPostffix { get; private set; }

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
