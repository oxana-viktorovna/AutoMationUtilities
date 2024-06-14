using System;

namespace ADOCore.Models
{

    public class TestPointResponse
    {
        public TestPoint[] value { get; set; }
        public int count { get; set; }
    }

    public class TestPoint
    {
        public int id { get; set; }
        public Tester tester { get; set; }
        public TestPointConfiguration configuration { get; set; }
        public bool isAutomated { get; set; }
        public Project project { get; set; }
        public Testplan testPlan { get; set; }
        public Testsuite testSuite { get; set; }
        public Lastupdatedby lastUpdatedBy { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public Results results { get; set; }
        public DateTime lastResetToActive { get; set; }
        public bool isActive { get; set; }
        public TestPointLinks links { get; set; }
        public Testcasereference testCaseReference { get; set; }
    }


    public class TestPointConfiguration
    {
        public int id { get; set; }
        public string name { get; set; }
    }


    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public string visibility { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Testplan
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Testsuite
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Results
    {
        public Lastresultdetails lastResultDetails { get; set; }
        public int lastResultId { get; set; }
        public string lastRunBuildNumber { get; set; }
        public string state { get; set; }
        public string lastResultState { get; set; }
        public string outcome { get; set; }
        public int lastTestRunId { get; set; }
    }

    public class Lastresultdetails
    {
        public int duration { get; set; }
        public DateTime dateCompleted { get; set; }
        public Runby runBy { get; set; }
    }

    public class Runby
    {
        public object displayName { get; set; }
        public string id { get; set; }
    }

    public class TestPointLinks
    {
        public _Self _self { get; set; }
        public Sourceplan sourcePlan { get; set; }
        public Sourcesuite sourceSuite { get; set; }
        public Sourceproject sourceProject { get; set; }
        public Testcases testCases { get; set; }
        public Run run { get; set; }
        public Result result { get; set; }
    }

    public class _Self
    {
        public string href { get; set; }
    }

    public class Sourceplan
    {
        public string href { get; set; }
    }

    public class Sourcesuite
    {
        public string href { get; set; }
    }

    public class Sourceproject
    {
        public string href { get; set; }
    }

    public class Testcases
    {
        public string href { get; set; }
    }

    public class Testcasereference
    {
        public int id { get; set; }
        public string name { get; set; }
        public string state { get; set; }
    }

}
