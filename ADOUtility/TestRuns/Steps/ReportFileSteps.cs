using ADOCore.Models;
using SharedCore.FileUtilities;
using TestRuns.Models;
using TestRuns.Utilities;

namespace TestRuns.Steps
{
    public class ReportFileSteps
    {
        private const string CsvFormat = ".csv";
        private const string DefaultUiFileName = "FailedUITests_";
        private const string DefaultApiFileName = "FailedAPITests_";

        public void SaveUiResults(string filePath, string fileName, IEnumerable<ResultReport> testResults)
        {
            var csvWorker = new CsvWorker(Path.Combine(filePath, DefaultUiFileName + fileName + CsvFormat));

            var fileContent = testResults.Where(result => result != null).ToList().ToCsvFormat(csvWorker.Splitter);
            csvWorker.Write(fileContent);
        }

        public void SaveAllUiResults(string filePath, string fileName, IEnumerable<TestRunUnitTestResult> testResults)
        {
            var csvWorker = new CsvWorker(Path.Combine(filePath, DefaultUiFileName + fileName + CsvFormat));

            var fileContent = testResults.ToList().ToCsvFormat(csvWorker.Splitter);
            csvWorker.Write(fileContent);
        }

        public void SaveUiPassedResultsWithDurationComapre(string filePath, string fileName, IEnumerable<(TestRunUnitTestResult currResult, DateTime preDuration)> durationComparer, string preBuildNumber)
        {
            var csvWorker = new CsvWorker(Path.Combine(filePath, DefaultUiFileName + fileName + CsvFormat));

            var fileContent = durationComparer.ToList().ToCsvFormat(csvWorker.Splitter, preBuildNumber);
            csvWorker.Write(fileContent);
        }

        public void SaveUiPassedResultsWithDuration(string filePath, string fileName, IEnumerable<TestRunUnitTestResult> duration)
        {
            var csvWorker = new CsvWorker(Path.Combine(filePath, DefaultUiFileName + fileName + CsvFormat));

            var fileContent = duration.ToList().ToCsvFormatWithDuration(csvWorker.Splitter);
            csvWorker.Write(fileContent);
        }

        public void SaveApiFailedResults(string filePath, string buildNum, IEnumerable<(string apiName, string request, string test)> testResults)
        {
            if (!testResults.Any())
                return;

            var csvWorker = new CsvWorker(Path.Combine(filePath, DefaultApiFileName + buildNum + CsvFormat));

            var fileContent = testResults.Select(r => csvWorker.Splitter + r.apiName + csvWorker.Splitter + r.request + csvWorker.Splitter+r.test).ToList();
            fileContent.Insert(0, "N, API Name, Request, Test, Comment");
            csvWorker.Write(fileContent);
        }

        public List<ResultReport>? ReadUiReportResult(string filePath, string runTitle)
        {
            var csvWorker = new CsvWorker(Path.Combine(filePath, runTitle + CsvFormat));
            try
            {
                var csvData = csvWorker.Read();
                var reportResults = ResultReportConverter.Convert(csvData);
                
                return reportResults;
            }
            catch (FileNotFoundException ex)
            {
                return null;
            }

        }

        public List<ResultReport> CompareResultsWithBlockers(string filePath, string preFileName, List<ResultReport> currResults)
        {
            var previousResults = ReadUiReportResult(filePath, preFileName);
            if (previousResults == null)
                return currResults;

            return ResultReportComparer.CopyBlockedComments(currResults, previousResults);
        }
    }
}
