using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCore.Models
{

    public class TestSuiteTests
    {
        public Value[] value { get; set; }
        public int count { get; set; }
    }

    public class Value
    {
        public Testcase testCase { get; set; }
        public Pointassignment[] pointAssignments { get; set; }
    }

    public class Testcase
    {
        public string id { get; set; }
        public string url { get; set; }
        public string webUrl { get; set; }
    }

    public class Pointassignment
    {
        public Configuration configuration { get; set; }
        public Tester tester { get; set; }
    }

    public class Configuration
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Tester
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
        public bool inactive { get; set; }
    }
}
