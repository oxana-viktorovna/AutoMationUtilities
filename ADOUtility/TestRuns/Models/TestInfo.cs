using System.Text;

namespace TestRuns.Models
{
    public class TestInfo
    {
        public TestInfo(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public static class TestInfoExtensions
    {
        public static string ToCsvLine(this TestInfo testInfo, string splitter)
            => testInfo.Id + splitter + testInfo.Name;
        public static StringBuilder ToCsvFormat(this IEnumerable<TestInfo> testInfos, string splitter)
        {
            var strbuilder = new StringBuilder();
            var headers = new List<string>()
           {
               "Id",
               "Name"
           };
            strbuilder.AppendLine(string.Join(splitter, headers));
            foreach (var testInfo in testInfos)
            {
                strbuilder.AppendLine(testInfo.ToCsvLine(splitter));
            }
            return strbuilder;
        }
    }
}