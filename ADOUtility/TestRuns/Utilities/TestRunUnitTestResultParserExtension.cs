using ADOCore.Models;
using SharedCore.StringUtilities;
using System.Text;

namespace TestRuns.Utilities
{
    public static class TestRunUnitTestResultParserExtension
    {
        public static StringBuilder ToCsvFormat(this List<TestRunUnitTestResult> testResults, string splitter)
        {
            var csv = new StringBuilder();
            var headers = new List<string>()
            {
            "N",
            "Reason",
            "Test Case N",
            "Test Method Name",
            "Error"
            };
            csv.AppendLine(string.Join(splitter, headers));

            for (int i = 0; i < testResults.Count; i++)
            {
                string error = string.Empty;
                try
                {
                    error = testResults[i].Output.ErrorInfo.Message.Trim().Replace(',','-').Replace("\r\n",". ");
                }
                catch { }

                csv.AppendLine(i + 1 + splitter +
                    "" + splitter +
                testResults[i].testName.GetTestCaseNumber() + splitter +
                testResults[i].testName.GetTestMethodName() + splitter +
                error
                );
            }

            return csv;
        }

        public static StringBuilder ToCsvFormatWithDuration(this List<TestRunUnitTestResult> testResults, string splitter)
        {
            var csv = new StringBuilder();
            var headers = new List<string>()
            {
            "N",
            "Test Case N",
            "Test Method Name",
            "Error",
            "Duration"
            };
            csv.AppendLine(string.Join(splitter, headers));

            for (int i = 0; i < testResults.Count; i++)
            {
                string error = string.Empty;
                try
                {
                    error = testResults[i].Output.ErrorInfo.Message.Trim().Replace(',', '-').Replace("\r\n", ". ");
                }
                catch { }

                csv.AppendLine(i + 1 + splitter +
                testResults[i].testName.GetTestCaseNumber() + splitter +
                testResults[i].testName.GetTestMethodName() + splitter +
                error + splitter +
                testResults[i].duration
                );
            }

            return csv;
        }

        public static StringBuilder ToCsvFormat(this List<(TestRunUnitTestResult testResults, DateTime preDuration)> durationComparer, string splitter, string preBuildNumber)
        {
            var csv = new StringBuilder();
            var headers = new List<string>()
            {
            "Test Case N",
            "Test Method Name",
            "Current Duration",
            $"Previous Duration {preBuildNumber}",
            "Duration Dirrerence"
            };
            csv.AppendLine(string.Join(splitter, headers));

            for (int i = 0; i < durationComparer.Count; i++)
            {
                var symbol = DateTime.Compare(durationComparer[i].testResults.duration, durationComparer[i].preDuration)
                    > 0 ? "+" : "-";
                var diff = durationComparer[i].testResults.duration - durationComparer[i].preDuration;
                csv.AppendLine(
                    durationComparer[i].testResults.testName.GetTestCaseNumber() + splitter +
                    durationComparer[i].testResults.testName.GetTestMethodName() + splitter +
                    durationComparer[i].testResults.duration.ToString("HH:mm:ss") + splitter +
                    durationComparer[i].preDuration.ToString("HH:mm:ss") + splitter +
                    symbol+diff.ToString(@"mm\:ss")
                ) ;
            }

            return csv;
        }
    }
}
