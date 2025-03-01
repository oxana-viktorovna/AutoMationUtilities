using SharedCore.Settings;

namespace ADOCore
{
    public class AdoSettings
    {
        public AdoSettings(SettingsReader reader)
        {
            var organization = reader.GetSetting("organization");
            Project = reader.GetSetting("project");
            UserName = reader.GetSetting("Auth:UserName");
            Password = reader.GetSetting("Auth:Password");
            BaseOrgUrl = $"https://dev.azure.com/{organization}";
            BaseUrlAPI = $"{BaseOrgUrl}/{Project}/_apis";
        }
        public string BaseOrgUrl { get; private set; }

        public string BaseUrlAPI { get; private set; }

        public string Project { get; private set; }

        public string UserName { get; private set; }

        public string Password { get; private set; }
    }
}
