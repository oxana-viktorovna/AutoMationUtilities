using System;
using System.Collections.Generic;
using System.Text;

namespace TestRuns.Models
{
    public record RunSummary
    {
        public int Passed { get; set; }
        public int PassedOnRerun { get; set; }
        public int NotExecuted { get; set; }
        public int Failed { get; set; }
    }
}
