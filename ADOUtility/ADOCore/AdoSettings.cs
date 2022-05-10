using SharedCore.Settings;

namespace ADOCore
{
    public class AdoSettings
    {
        public AdoSettings(SettingsReader reader)
        {
            var organization = reader.GetSetting("organization");
            var project = reader.GetSetting("project");
            UserName = reader.GetSetting("Auth:UserName");
            Password = reader.GetSetting("Auth:Password");
            BaseUrl = $"https://dev.azure.com/{organization}/{project}/_apis";
        }
        public string BaseUrl { get; private set; }

        public string UserName { get; private set; }

        public string Password { get; private set; }
    }
}
