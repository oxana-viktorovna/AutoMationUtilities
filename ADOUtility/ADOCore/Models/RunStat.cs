namespace ADOCore.Models
{
    public class RunStat
    {
        public Run run { get; set; }
        public Runstatistic[] runStatistics { get; set; }
    }

    public class Run
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Runstatistic
    {
        public string state { get; set; }
        public string outcome { get; set; }
        public int count { get; set; }
        public string resultMetadata { get; set; }
    }
}
