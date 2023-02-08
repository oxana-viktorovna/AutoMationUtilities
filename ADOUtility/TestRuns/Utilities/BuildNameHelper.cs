namespace TestRuns.Utilities
{
    public class BuildNameHelper
    {
        public BuildNameHelper(string buildName)
        {
            this.buildName = buildName;
            environmets = new List<string>()
            {
                "lzhighlander",
                "lzushammers",
                "lzushammers2",
                "lzusstaging",
                "lzustest",
                "lzustest02",
                "lzustest03",
                "lzustest04",
                "lzustest3",
                "testauto",
                "testauto3",
                "ukint",
                "uknewinf",
                "ukqa1testauto",
                "ukqatestauto",
                "ukstaging",
                "usbrest2",
                "ushighland",
                "usint",
                "usmilkyway",
                "usnewinf",
                "usplatform",
                "usplatform2sa",
                "usqa1testauto",
                "usqatestauto",
                "usracoons2",
                "usstaging"
            };

            browsers = new List<string>()
            { 
                "chrome",
                "edge"
            };

            SplitBuildName();
        }

        public string? Env { get; private set; }

        public string? Browser { get; private set; }

        private readonly string buildName; 

        private readonly List<string> environmets;

        private readonly List<string> browsers;


        private void SplitBuildName()
        {
            foreach (var environmet in environmets)
            {
                if (buildName.ToLower().Contains($"-{environmet}-"))
                    Env = environmet;               
            }
            foreach (var browser in browsers)
            {
                if (buildName.ToLower().Contains(browser))
                    Browser = browser;
            }
        }
    }
}
