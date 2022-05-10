using Newtonsoft.Json;

namespace ADOCore.Models
{
    internal class ResultDetailsByBuildResponce
    {
        public string groupByField { get; set; }
        public Resultsforgroup[] resultsForGroup { get; set; }
    }

    public class Resultsforgroup
    {
        public string groupByValue { get; set; }
        public Resultscountbyoutcome resultsCountByOutcome { get; set; }
        public Result[] results { get; set; }
    }

    public class Resultscountbyoutcome
    {
        public Passed Passed { get; set; }
        public Failed Failed { get; set; }
    }

    public class Passed
    {
        public string outcome { get; set; }
        public int count { get; set; }
        public string duration { get; set; }
    }

    public class Failed
    {
        public string outcome { get; set; }
        public int count { get; set; }
        public string duration { get; set; }
    }

    public class Result
    {
        public int id { get; set; }

        [JsonProperty("Project")]
        public ProjectResultDetailsByBuild project { get; set; }
        public float durationInMs { get; set; }
        public string outcome { get; set; }
        public Testrun testRun { get; set; }
        public int priority { get; set; }
        public int testCaseReferenceId { get; set; }
    }
   
    public class ProjectResultDetailsByBuild
    {
        public string id { get; set; }
    }

    public class Testrun
    {
        public string id { get; set; }
    }

}
