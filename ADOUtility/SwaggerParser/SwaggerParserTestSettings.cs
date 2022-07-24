using SharedCore.Settings;

namespace SwaggerParser
{
    public class SwaggerParserTestSettings
    {
        public SwaggerParserTestSettings(SettingsReader reader)
        {
            SpecFolder = reader.GetSetting("specFolder");
            CollFolder= reader.GetSetting("collFolder");
        }

        public string SpecFolder { get; private set; }

        public string CollFolder { get; private set; }
    }
}
