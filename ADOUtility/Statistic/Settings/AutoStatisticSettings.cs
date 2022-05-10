using SharedCore.Settings;
using System;
using System.Collections.Generic;

namespace Statistic.Settings
{
    internal class AutoStatisticSettings
    {
        public AutoStatisticSettings(SettingsReader reader)
        {
            FirstTimeRun = Convert.ToBoolean(reader.GetSetting("firstTimeRun"));
            SaveFolder = reader.GetSetting("saveFolder");
            DefaultAreaPathes = reader.GetSettingArray<string>("defaultAreaPathes");
            SetAsOf(reader);
        }

        public bool FirstTimeRun { get; private set; }
        public string SaveFolder { get; private set; }
        public List<string> DefaultAreaPathes { get; private set; }
        public DateTime AsOf { get; set; }

        private void SetAsOf(SettingsReader reader)
        {
            var asOfStr = reader.GetSetting("asOfDate");
            AsOf = string.IsNullOrEmpty(asOfStr) 
                ? DateTime.Now 
                : Convert.ToDateTime(asOfStr);
        }
    }
}
