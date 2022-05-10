using Newtonsoft.Json;
using System;

namespace ADOCore.Models
{
    public class GetBuildResponce
    {
        [JsonProperty("_links")]
        public BuildLinks links { get; set; }
        public Properties properties { get; set; }
        public object[] tags { get; set; }
        public object[] validationResults { get; set; }
        public Plan[] plans { get; set; }
        public Triggerinfo triggerInfo { get; set; }
        public int id { get; set; }
        public string buildNumber { get; set; }
        public string status { get; set; }
        public string result { get; set; }
        public DateTime queueTime { get; set; }
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public string url { get; set; }
        public Definition definition { get; set; }
        public int buildNumberRevision { get; set; }

        [JsonProperty("project")]
        public BuildProject project { get; set; }
        public string uri { get; set; }
        public string sourceBranch { get; set; }
        public string sourceVersion { get; set; }
        public Queue queue { get; set; }
        public string priority { get; set; }
        public string reason { get; set; }
        public Requestedfor requestedFor { get; set; }
        public Requestedby requestedBy { get; set; }
        public DateTime lastChangedDate { get; set; }
        public Lastchangedby lastChangedBy { get; set; }
        public Orchestrationplan orchestrationPlan { get; set; }
        public Logs logs { get; set; }
        public Repository repository { get; set; }
        public bool retainedByRelease { get; set; }
        public object triggeredByBuild { get; set; }
    }

    public class BuildLinks
    {
        public Self self { get; set; }
        public Web web { get; set; }
        public Sourceversiondisplayuri sourceVersionDisplayUri { get; set; }
        public Timeline timeline { get; set; }
        public Badge badge { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Web
    {
        public string href { get; set; }
    }

    public class Sourceversiondisplayuri
    {
        public string href { get; set; }
    }

    public class Timeline
    {
        public string href { get; set; }
    }

    public class Badge
    {
        public string href { get; set; }
    }

    public class Properties
    {
    }

    public class Triggerinfo
    {
        public object scheduleName { get; set; }
    }

    public class Definition
    {
        public object[] drafts { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string uri { get; set; }
        public string path { get; set; }
        public string type { get; set; }
        public string queueStatus { get; set; }
        public int revision { get; set; }
        public BuildProject project { get; set; }
    }

    public class BuildProject
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public int revision { get; set; }
        public string visibility { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Queue
    {
        public int id { get; set; }
        public string name { get; set; }
        public Pool pool { get; set; }
    }

    public class Pool
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Requestedfor
    {
        public string displayName { get; set; }
        public string url { get; set; }
        [JsonProperty("_links")]
        public Links links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class Requestedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        [JsonProperty("_links")]
        public Links links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class Lastchangedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        [JsonProperty("_links")]
        public Links links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class Orchestrationplan
    {
        public string planId { get; set; }
    }

    public class Logs
    {
        public int id { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }

    public class Repository
    {
        public string id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public object clean { get; set; }
        public bool checkoutSubmodules { get; set; }
    }

    public class Plan
    {
        public string planId { get; set; }
    }
}
