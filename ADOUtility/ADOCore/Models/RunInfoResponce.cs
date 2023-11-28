using Newtonsoft.Json;
using System;

namespace ADOCore.Models
{
    public class RunInfoResponce
    {
        [JsonProperty("Value")]
        public RunInfo[] value { get; set; }
        public int count { get; set; }
    }

    public class RunInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public Buildconfiguration buildConfiguration { get; set; }
        public bool isAutomated { get; set; }
        public Owner owner { get; set; }

        [JsonProperty("project")]
        public RunInfoProject1 project { get; set; }
        public DateTime startedDate { get; set; }
        public DateTime completedDate { get; set; }
        public string state { get; set; }
        public int totalTests { get; set; }
        public int incompleteTests { get; set; }
        public int notApplicableTests { get; set; }
        public int passedTests { get; set; }
        public int unanalyzedTests { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public Lastupdatedby lastUpdatedBy { get; set; }
        public int revision { get; set; }
        public Release release { get; set; }
        public Runstatistic[] runStatistics { get; set; }
        public string webAccessUrl { get; set; }
        public Pipelinereference pipelineReference { get; set; }
    }

    public class Buildconfiguration
    {
        public int id { get; set; }
        public string number { get; set; }
        public string flavor { get; set; }
        public string platform { get; set; }
        public int buildDefinitionId { get; set; }
        [JsonProperty("project")]
        public RunInfoProject project { get; set; }
    }

    public class RunInfoProject
    {
        public string name { get; set; }
    }

    public class Owner
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

    public class Links
    {
        public Avatar avatar { get; set; }
        public LinkDetails TestPoints { get; set; }
        public LinkDetails Configuration { get; set; }
        public LinkDetails Self { get; set; }
        public LinkDetails SourcePlan { get; set; }
        public LinkDetails SourceSuite { get; set; }
        public LinkDetails SourceProject { get; set; }
        public TestCases TestCases { get; set; }
    }

    public class Avatar
    {
        public string href { get; set; }
    }

    public class RunInfoProject1
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Lastupdatedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public BuildLinks _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class Release
    {
        public int id { get; set; }
        public object name { get; set; }
        public int environmentId { get; set; }
        public object environmentName { get; set; }
        public int definitionId { get; set; }
        public int environmentDefinitionId { get; set; }
        public object environmentDefinitionName { get; set; }
    }

    public class Pipelinereference
    {
        public int pipelineId { get; set; }
        public Stagereference stageReference { get; set; }
        public Phasereference phaseReference { get; set; }
        public Jobreference jobReference { get; set; }
    }

    public class Stagereference
    {
        public string stageName { get; set; }
    }

    public class Phasereference
    {
        public string phaseName { get; set; }
    }

    public class Jobreference
    {
        public string jobName { get; set; }
    }
}
