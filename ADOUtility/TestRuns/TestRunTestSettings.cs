using SharedCore.Settings;
using System;

namespace TestRuns
{
    public class TestRunTestSettings
    {
        public TestRunTestSettings(SettingsReader reader)
        {
            SaveFolder = reader.GetSetting("saveFolder");
            CurrBuildId = Convert.ToInt32(reader.GetSetting("currentTestRun:buildId"));
            RunDuration = reader.GetSetting("currentTestRun:runDuration");
            PreviousBuildId = GetPreviousBuildId(reader);
        }

        public string SaveFolder { get; private set; }
        public int CurrBuildId { get; private set; }
        public string RunDuration { get; private set; }
        public int PreviousBuildId { get; private set; }

        private int GetPreviousBuildId(SettingsReader reader)
        { 
            var buildIdStr = reader.GetSetting("previousTestRun:buildId");

            return string.IsNullOrEmpty(buildIdStr)
                ? 0
                : Convert.ToInt32(buildIdStr);
        }
    }
}
