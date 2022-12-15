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
            headerRow.CreateCell(1, "Reason", style);
            headerRow.CreateCell(2, "TestCase N", style);
            headerRow.CreateCell(3, "Test Method", style);
            headerRow.CreateCell(4, "Error", style);
            
        }

        private void CreateFailedUiList(List<ResultReport> testResults)
        {
            var style = stylesBuilder.GetRegularBottomBorderStyle();
            testResults = testResults.OrderBy(result => result.Reason).ThenBy(result => result.Error).ToList();
            for (int i = 0; i < testResults.Count; i++)
            {
                var row = failedUiSheet.CreateRow(i + 1);
                row.CreateCell(0, i + 1, style);
                row.CreateCell(1, testResults[i].Reason, style);
                row.CreateCell(2, Convert.ToInt32(string.IsNullOrEmpty(testResults[i].TestCaseNumber) ? "0" : testResults[i].TestCaseNumber), style);
                row.CreateCell(3, testResults[i].TestMethodName, style);
                row.CreateCell(4, testResults[i].Error, style);
            }
        }
    }
}
