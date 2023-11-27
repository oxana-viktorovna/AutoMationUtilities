using NPOI.SS.UserModel;
using SharedCore.FileUtilities.Excel;
using TestCases.Models;

namespace TestCases.Utilities
{
    public class RunNewUiSummaryBuilder : ExcelWorker
    {
        public RunNewUiSummaryBuilder(IWorkbook book)
        {
            this.book = book;
            stylesBuilder = new ExcelStylesCreater(book);
        }

        private ISheet failedUiSheet;
        private readonly IWorkbook book;
        private readonly ExcelStylesCreater stylesBuilder;
        public void CreateTestIdsReport(List<TestInfo> testInfo)
        {
            CreateTestIdHeaders();
            CreateTestIdList(testInfo);
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
    }
}
