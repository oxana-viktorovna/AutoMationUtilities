using System.Collections.Generic;
using System.Linq;
using TestRuns.Models;

namespace TestRuns.Utilities
{
    public static class ResultReportConverter
    {
        public static List<ResultReport> Convert(List<string[]> datas)
        {
            datas.RemoveAt(0);

            return datas.Select(data => Convert(data)).ToList();
        }

        public static ResultReport Convert(string[] data)
            => new ResultReport(
                System.Convert.ToInt32(data[0]), 
                data[1], 
                data[2], 
                data[3].TrimStart('"').TrimEnd('"'), 
                data[4]);
    }
}
