using TestCases.Models;

namespace TestCases.Utilities
{
    public class ResultReportConverter
    {
        public static List<TestInfo> ConvertToTestInfo(List<List<(string Id, string Name)>> batches)
        {
            List<TestInfo> testInfoList = new List<TestInfo>();
            foreach (var batch in batches)
            {
                foreach (var pair in batch)
                {
                    testInfoList.Add(new TestInfo(pair.Id, pair.Name));
                }
            }
            return testInfoList;
        }
    }
}
