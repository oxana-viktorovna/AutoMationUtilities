using System.IO;
using System.Reflection;

namespace Statistic.Utilities
{
    internal static class PathResolver
    {
        internal static string GetReadPath(bool firstTimeRun, string localFolder)
        {
            if (firstTimeRun)
            {
                var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Combine(assemblyPath, "StatisticResources", "AutoUIStatisticInitial.xlsx");
            }

            return Path.Combine(localFolder, "AutoUIStatistic.xlsx");
        }

        internal static string GetSavePath(string localFolder)
            => Path.Combine(localFolder, "AutoUIStatistic.xlsx");
    }
}
