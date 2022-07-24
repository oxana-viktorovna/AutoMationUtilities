using NPOI.SS.UserModel;
using SharedCore.FileUtilities.Excel;

namespace TestRuns.Utilities
{
    public class RunNewApiSummaryBuilder: ExcelWorker
    {
        public RunNewApiSummaryBuilder(IWorkbook book)
        {
            this.book = book;
            stylesBuilder = new ExcelStylesCreater(book);
        }

        private ISheet failedApiSheet;
        private readonly IWorkbook book;
        private readonly ExcelStylesCreater stylesBuilder;

        public void CreateFullFailedApiReport(List<(string apiName, string request, string test)> failedResults)
        {
            failedApiSheet = book.CreateSheet("API Failed");
            CreateFailedApiHeaders();
            CreateFailedApiList(failedResults);
        }

        private void CreateFailedApiHeaders()
        {
            var headerRow = failedApiSheet.CreateRow(0);
            var style = stylesBuilder.GetHeaderFullBorderStyle();
            headerRow.CreateCell(0, "N", style);
            headerRow.CreateCell(1, "API Name", style);
            headerRow.CreateCell(2, "Request", style);
            headerRow.CreateCell(3, "Test", style);
            headerRow.CreateCell(4, "Comment", style);
        }

        private void CreateFailedApiList(List<(string apiName, string request, string test)> testResults)
        {
            var style = stylesBuilder.GetRegularFullBorderStyle();

            for (int i = 0; i < testResults.Count; i++)
            {
                var row = failedApiSheet.CreateRow(i + 1);
                row.CreateCell(0, i + 1, style);
                row.CreateCell(1, testResults[i].apiName, style);
                row.CreateCell(2, testResults[i].request, style);
                row.CreateCell(3, testResults[i].test, style);
            }
        }
    }
}
