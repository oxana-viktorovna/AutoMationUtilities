using SharedCore.StringUtilities;
using System.Collections.Generic;
using System.Linq;
using TestRuns.Models;

namespace TestRuns.Utilities
{
    public static class ResultReportComparer
    {
        public static List<ResultReport> CopyComments(
            List<ResultReport> currentResults,
            List<ResultReport> previousResults)
        {
            var previousResultWithComment = previousResults.Where(result => !result.Comment.Equals(TestRunConstants.DefaultComment));

            foreach (var previousResult in previousResultWithComment)
            {
                var sameName = currentResults
                    .Where(result => result.TestMethodName.Equals(previousResult.TestMethodName));
                var sameResult = sameName.Where(result => LevenshteinDistance.Calculate(result.Error.Replace("\"", ""), previousResult.Error.Replace("\"", "")) < 15);

                if (sameResult.Any())
                {
                    sameResult.First().Comment = previousResult.Comment;
                }
            }

            return currentResults;
        }
    }
}
