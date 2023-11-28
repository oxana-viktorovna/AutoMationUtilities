using System;
using System.Collections.Generic;
namespace ADOCore.Models
{
    public class TestPlanDetailsWithNestedSuits 
    {
        public int id { get; set; }
        public int revision { get; set; }
        public Project project { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public Plan plan { get; set; }
        public List<ChildSuite> children { get; set; }
        public bool hasChildren { get; set; }
        public Links links { get; set; }
        public string suiteType { get; set; }
        public string name { get; set; }
        public bool inheritDefaultConfigurations { get; set; }
        public List<DefaultConfiguration> defaultConfigurations { get; set; }
    }
    public class Plans
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class ChildSuite
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class TestCases
    {
        public string href { get; set; }
    }
    public class TestPoints
    {
        public string href { get; set; }
    }
    public class DefaultConfiguration
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}