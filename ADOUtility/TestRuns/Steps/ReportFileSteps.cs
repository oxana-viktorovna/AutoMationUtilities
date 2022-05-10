using ADOCore.Models;
using SharedCore.FileUtilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestRuns.Models;
using TestRuns.Utilities;

namespace TestRuns.Steps
{
    public class ReportFileSteps
    {
        private const string CsvFormat = ".csv";
        private const string DefaultUiFileName = "FailedUITests_";
        private const string DefaultApiFileName = "FailedAPITests_";

        public void SaveUiFailedResults(string filePath, string buildNum, IEnumerable<TestRunUnitTestResult> testResults)
        {
            var csvWorker = new CsvWorker(Path.Combine(filePath, DefaultUiFileName + buildNum + CsvFormat));
            var orderedResults = testResults.OrderBy(result => result.Output.ErrorInfo.Message).ToList();

            var fileContent = orderedResults.ToCsvFormat(csvWorker.Splitter);
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
            var csvData = csvWorker.Read();
            var reportResults = ResultReportConverter.Convert(csvData);

            return reportResults;
        }

        public void CompareResultsWithPrevious(string filePath, string preBuildNum, string currBuildNum)
        {
            if (string.IsNullOrEmpty(preBuildNum))
                return;

            var previousResults = ReadUiReportResult(filePath, DefaultUiFileName + preBuildNum);
            var currentResults = ReadUiReportResult(filePath, DefaultUiFileName + currBuildNum);
            var currentResultsWithComments = ResultReportComparer.CopyComments(currentResults, previousResults);

            SaveResultReport(filePath, DefaultUiFileName + currBuildNum, currentResultsWithComments);
        }
    }
}
