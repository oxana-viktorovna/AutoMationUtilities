using System;
using System.Collections.Generic;
namespace ADOCore.Models
{
    public class TestPlanDetailsWithoutNestedSuits
    {
        public List<TestPlanDetailItem> value { get; set; }
    }
    public class TestPlanDetailItem
    {
        public int id { get; set; }
        public int revision { get; set; }
        public Project project { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public Plan plan { get; set; }
        public bool hasChildren { get; set; }
        public Links links { get; set; }
        public string suiteType { get; set; }
        public string name { get; set; }
        public bool inheritDefaultConfigurations { get; set; }
        public List<DefaultConfiguration> defaultConfigurations { get; set; }
    }
}