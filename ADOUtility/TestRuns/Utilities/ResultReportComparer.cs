using ADOCore.Models;
using SharedCore.StringUtilities;
using TestRuns.Models;

namespace TestRuns.Utilities
{
    public static class ResultReportComparer
    {
        public static List<ResultReport> CopyComments(
            List<ResultReport> currentResults,
            List<ResultReport> previousResults)
        {
            var previousResultWithComment = previousResults.Where(result => !result.Reason.Equals(TestRunConstants.DefaultComment));

            foreach (var previousResult in previousResultWithComment)
            {
                var sameName = currentResults
                    .Where(result => result.TestMethodName.Equals(previousResult.TestMethodName));
                var sameResult = sameName.Where(result => LevenshteinDistance.Calculate(result.Error.Replace("\"", ""), previousResult.Error.Replace("\"", "")) < 15);

                if (sameResult.Any())
                    sameResult.First().Reason = previousResult.Reason;
            }

            return currentResults;
        }

        public static List<ResultReport> CopyBlockedComments(
            List<ResultReport> currentResults,
            List<ResultReport> previousResults)
        {
            var preNames = previousResults.Select(p => p.TestMethodName);
            foreach (var currentResult in currentResults)
            {
                if (preNames.Contains(currentResult.TestMethodName))
                    currentResult.Reason = previousResults.Where(p => p.TestMethodName.Equals(currentResult.TestMethodName)).First().Reason;
            }

            return currentResults;
        }

        public static List<ResultReport> CopyCommentsIgnoreError(
            List<ResultReport> currentResults,
            List<ResultReport> previousResults)
        {
            foreach (var previousResult in previousResults)
            {
                var sameName = currentResults
                    .Where(result => result.TestMethodName.Equals(previousResult.TestMethodName));

                if (sameName.Any())
                    sameName.First().Reason = previousResult.Reason;
            }

            return currentResults;
        }
    }
}
