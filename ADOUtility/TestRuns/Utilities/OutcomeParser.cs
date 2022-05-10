using ADOCore.Models;
using System.Collections.Generic;
using System.Text;

namespace TestRuns.Utilities
{
    public static class OutcomeParser
    {
        public static StringBuilder ToCsvFormat(this List<OutcomeResult> outcomes, string splitter)
        {
            var csv = new StringBuilder();
            var headers = new List<string>()
            {
            "Test Case N",
            "Test Method Name",
            "Outcome"
            };
            csv.AppendLine(string.Join(splitter, headers));

            foreach (var outcome in outcomes)
            {
                var line = new List<string>()
                {
                outcome.TestId,
                outcome.TestName,
                outcome.Outcome
                };
                csv.AppendLine(string.Join(splitter, line));
            }

            return csv;
        }
    }
}
