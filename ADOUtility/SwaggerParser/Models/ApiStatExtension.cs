using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwaggerParser.Models
{
    public static class ApiStatExtension
    {
        public static string ApiStatsToCvsString(this List<ApiStat> apiStats)
        {
            var splitter = ",";
            var cvsStr = new StringBuilder();
            var apiStatsOrdered = apiStats.OrderBy(s => s.ApiName).ThenBy(s => s.Tag).ThenBy(s => s.Endpoint).ThenBy(s => s.Method);
            foreach (var apiStat in apiStatsOrdered)
            {
                cvsStr.AppendLine($"{apiStat.ApiName}{splitter}{apiStat.Tag}{splitter}{apiStat.Method}{splitter}{apiStat.Endpoint}");
            }

            return cvsStr.ToString();
        }
    }
}