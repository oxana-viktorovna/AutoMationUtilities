using ADOCore.Models;
using System.Collections.Generic;
using System.Linq;
using TestRuns.Models;

namespace TestRuns.Utilities
{
    public static class RunStatParser
    {
        public static RunSummary GetApiStatistic(this IEnumerable<RunStat> runStats)
        {
            var apiStats = runStats.Where(rstst => rstst.run.name.Contains(".xml"));
            var outcomeStat = GetOutcomeStat(apiStats);

            return new RunSummary
            {
                Passed = GetCountByOutcome(outcomeStat, "Passed"),
                Failed = GetCountByOutcome(outcomeStat, "Failed"),
                NotExecuted = GetCountByOutcome(outcomeStat, "NotExecuted"),
                PassedOnRerun = GetCountByOutcome(outcomeStat, "PassedOnReRun")
            };
        }

        public static RunSummary GetUiStatistic(this IEnumerable<RunStat> runStats)
        {
            var uiStats = runStats.Where(rstst => !rstst.run.name.Contains(".xml")); 
            var outcomeStat = GetOutcomeStat(uiStats);

            return new RunSummary
            {
                Passed = GetCountByOutcome(outcomeStat, "Passed"),
                Failed = GetCountByOutcome(outcomeStat, "Failed"),
                NotExecuted = GetCountByOutcome(outcomeStat, "NotExecuted"),
                PassedOnRerun = GetCountByOutcome(outcomeStat, "PassedOnReRun")
            };
        }

        public static List<Runstatistic> GetOutcomeStat(this IEnumerable<RunStat> runStats)
        {
            var runstatistic = runStats.SelectMany(rst => rst.runStatistics);
            var statistic = runstatistic.GroupBy(stat => stat.outcome).Select(gstat =>
            {
                var totalCount = gstat.Sum(st => st.count);
                return new Runstatistic()
                {
                    count = totalCount,
                    outcome = gstat.Key
                };
            }
                ).ToList();

            var rerun = runstatistic.Where(stat => stat.outcome == "Passed" && stat.resultMetadata=="rerun").Sum(stat => stat.count);

            statistic.Add(new Runstatistic()
            {
                count = rerun,
                outcome = "PassedOnReRun"
            });

            return statistic;
        }

        private static int GetCountByOutcome(List<Runstatistic> outcomeStat, string outcome)
            => outcomeStat.Where(s => s.outcome == outcome).Select(s => s.count).FirstOrDefault();
    }
}
