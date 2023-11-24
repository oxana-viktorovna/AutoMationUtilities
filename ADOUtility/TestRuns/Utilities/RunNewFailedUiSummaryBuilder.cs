using NPOI.SS.UserModel;
using SharedCore.FileUtilities.Excel;
using TestRuns.Models;

namespace TestRuns.Utilities
{
    public class RunNewUiSummaryBuilder: ExcelWorker
    {
        public RunNewUiSummaryBuilder(IWorkbook book)
        {
            this.book = book;
            stylesBuilder = new ExcelStylesCreater(book);
        }

        private ISheet failedUiSheet;
        private readonly IWorkbook book;
        private readonly ExcelStylesCreater stylesBuilder;

        public void CreateFullFailedUiReport(List<ResultReport> failedResults)
        {
            CreateFailedUiHeaders();
            CreateFailedUiList(failedResults);
        }

        public void CreateTestIdsReport(List<TestInfo> testInfo)
        {
            CreateTestIdHeaders();
            CreateFailedUiList(testInfo);
        }

        public void CreateFullFailedUiReport(List<ResultReport> failedResults, List<ResultReport> blockedBailedResults)
        {
            CreateFailedUiHeaders();
            if(blockedBailedResults != null)
                failedResults.AddRange(blockedBailedResults);
            CreateFailedUiList(failedResults);
        }

        private void CreateFailedUiHeaders()
        {
            failedUiSheet = book.CreateSheet("UI Failed"); 
            var headerRow = failedUiSheet.CreateRow(0);
            var style = stylesBuilder.GetHeaderBottomBorderStyle();
            headerRow.CreateCell(0, "N", style);
            headerRow.CreateCell(1, "Outcome", style);
            headerRow.CreateCell(2, "Reason", style);
            headerRow.CreateCell(3, "TestCase N", style);
            headerRow.CreateCell(4, "Test Method", style);
            headerRow.CreateCell(5, "Error", style);
        }

        private void CreateTestIdHeaders()
        {
            failedUiSheet = book.CreateSheet("Test Ids");
            var headerRow = failedUiSheet.CreateRow(0);
            var style = stylesBuilder.GetHeaderBottomBorderStyle();
            headerRow.CreateCell(0, "N", style);
            headerRow.CreateCell(1, "Name", style);
            headerRow.CreateCell(2, "Id", style);
        }

        private void CreateTestIdList(List<TestInfo> testResults)
        {
            var style = stylesBuilder.GetRegularBottomBorderStyle();
            testResults = testResults.OrderBy(result => result.Id).ThenBy(result => result.Name).ToList();
            for (int i = 0; i < testResults.Count; i++)
            {
                var id = testResults[i].Id;

                var row = failedUiSheet.CreateRow(i + 1);
                row.CreateCell(0, i + 1, style);
                row.CreateCell(1, id, style);
                row.CreateCell(2, testResults[i].Name, style);
            }
        }

        private void CreateFailedUiList(List<TestInfo> testInfo)
        {
            var style = stylesBuilder.GetRegularBottomBorderStyle();
            testInfo = testInfo.OrderBy(result => result.Name).ThenBy(result => result.Id).ToList();
            for (int i = 0; i < testInfo.Count; i++)
            {
                var id = testInfo[i].Id;

                var row = failedUiSheet.CreateRow(i + 1);
                row.CreateCell(0, i + 1, style);
                row.CreateCell(1, id, style);
                row.CreateCell(2, testInfo[i].Name, style);
            }
        }

        private void CreateFailedUiList(List<ResultReport> testResults)
        {
            var style = stylesBuilder.GetRegularBottomBorderStyle();
            testResults = testResults.OrderBy(result => result.Reason).ThenBy(result => result.Error).ToList();
            for (int i = 0; i < testResults.Count; i++)
            {
                var outcome = testResults[i].Outcome.Contains("Block") ? testResults[i].Outcome + " BLOCK": testResults[i].Outcome;

                var row = failedUiSheet.CreateRow(i + 1);
                row.CreateCell(0, i + 1, style);
                row.CreateCell(1, outcome, style);
                row.CreateCell(2, testResults[i].Reason, style);
                row.CreateCell(3, Convert.ToInt32(string.IsNullOrEmpty(testResults[i].TestCaseNumber) ? "0" : testResults[i].TestCaseNumber), style);
                row.CreateCell(4, testResults[i].TestMethodName, style);
                row.CreateCell(5, testResults[i].Error, style);
            }
        }
    }
}
