using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ADOCore.Models
{
    public class TestCaseResponce
    {
        [JsonProperty("value")]
        public List<TestPlanDetail> value { get; set; }
    }
    public class TestPlanDetail
    {
        public TestPlan testPlan { get; set; }
        public Project project { get; set; }
        public TestSuite testSuite { get; set; }
        public WorkItem workItem { get; set; }
        public List<PointAssignment> pointAssignments { get; set; }
        public Links links { get; set; }
    }
    public class TestPlan
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public string visibility { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }
    public class TestSuite
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class WorkItemField
    {
        public string fieldName { get; set; }
        public string value { get; set; }
    }
    public class PointAssignment
    {
        public int id { get; set; }
        public string configurationName { get; set; }
        public object tester { get; set; }
        public int configurationId { get; set; }
    }
    public class LinkDetails
    {
        public string href { get; set; }
    }
}
