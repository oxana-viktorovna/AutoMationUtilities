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

        public void SaveUiResults(string filePath, string fileName, IEnumerable<TestRunUnitTestResult> testResults, bool isFailed = true)
        {
            var csvWorker = new CsvWorker(Path.Combine(filePath, DefaultUiFileName + fileName + CsvFormat));
            if (isFailed)
                testResults = testResults.OrderBy(result => result.Output.ErrorInfo.Message);

            var fileContent = testResults.ToList().ToCsvFormat(csvWorker.Splitter);
            csvWorker.Write(fileContent);
        }

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

        public void SaveOutcomes(string filePath, string runTitle, IEnumerable<OutcomeResult> outcomes)
        {
            var csvWorker = new CsvWorker(Path.Combine(filePath, runTitle + CsvFormat));
            var orderedOutcomes = outcomes.OrderBy(outcome => outcome.TestId).ToList();

            var fileContent = orderedOutcomes.ToCsvFormat(csvWorker.Splitter);
            csvWorker.Write(fileContent);
        }

        public void SaveResultReport(string filePath, string runTitle, IEnumerable<ResultReport> resultReports)
        {
            var csvWorker = new CsvWorker(Path.Combine(filePath, runTitle + CsvFormat));
            var fileContent = resultReports.ToCsvFormat(csvWorker.Splitter);
            csvWorker.Write(fileContent);
        }

        public List<ResultReport> ReadUiReportResult(string filePath, string runTitle)
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

        public void CompareResultsWithPrevious(string filePath, string preFileName, string currFileName)
        {
            var previousResults = ReadUiReportResult(filePath, DefaultUiFileName + preFileName);
            if (previousResults == null)
                return;

            var currentResults = ReadUiReportResult(filePath, DefaultUiFileName + currFileName);
            var currentResultsWithComments = ResultReportComparer.CopyComments(currentResults, previousResults);

            SaveResultReport(filePath, DefaultUiFileName + currFileName, currentResultsWithComments);
        }

        public List<ResultReport> CompareResultsWithBlockers(string filePath, string preFileName, List<ResultReport> currResults)
        {
            var previousResults = ReadUiReportResult(filePath, preFileName);
            if (previousResults == null)
                return currResults;

            return ResultReportComparer.CopyBlockedComments(currResults, previousResults);
        }

        public void CompareResultsWithPreviousIgnoreError(string filePath, string preBuildNum, string currBuildNum)
        {
            var previousResults = ReadUiReportResult(filePath, DefaultUiFileName + preBuildNum);
            if (previousResults == null)
                return;

            var currentResults = ReadUiReportResult(filePath, DefaultUiFileName + currBuildNum);
            var currentResultsWithComments = ResultReportComparer.CopyCommentsIgnoreError(currentResults, previousResults);

            SaveResultReport(filePath, DefaultUiFileName + currBuildNum, currentResultsWithComments);
        }
    }
}
