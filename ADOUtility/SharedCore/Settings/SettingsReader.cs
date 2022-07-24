using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;

namespace SharedCore.Settings
{
    public class SettingsReader
    {
        public SettingsReader(string settingsFileName)
            => config = GetAppSettings(settingsFileName);

        private readonly IConfigurationRoot config;

        public IConfigurationRoot GetAppSettings(string settingsFileName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory
                .GetCurrentDirectory())
                .AddJsonFile(settingsFileName, optional: false, reloadOnChange: true);

            return builder.Build();
        }

        public string GetSetting(string settingName)
            => config[settingName];

        public T GetObjFromSection<T>(string settingName) where T : class
            => config.GetSection(settingName)
            .Get<T>();

        public List<T> GetSettingArray<T>(string section)
        { 
            var values = config.GetSection(section).Get<List<T>>();

            return values ?? new List<T>();
        }
    }
}
