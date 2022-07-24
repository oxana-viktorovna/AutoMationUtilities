using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SharedCore.FileUtilities.Excel;
using System.Reflection;
using TestRuns.Models;

namespace TestRuns.Utilities
{
    public class RunSummaryWorker : ExcelWorker
    {
        public RunSummaryWorker()
        {
        }

        public RunSummaryWorker(string pathToSave, string fileName)
        {
            saveFile = Path.Combine(pathToSave, DefaultSummaryFileName + fileName + XlsxFormat);
            savePath = pathToSave;
        }

        private const string XlsxFormat = ".xlsx";
        private const string DefaultSummaryFileName = "TestResultSummary_";
        private string saveFile;
        private string savePath;

        public void UpdateCurrentSummaryReport(RunSummary uiSummary, RunSummary apiSummary, string runDuration)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var book = Read(Path.Combine(assemblyPath, @"Resources\TestResultSummary" + XlsxFormat));
            ISheet sheet1 = book.GetSheetAt(0);
            var uiRow = sheet1.GetRow(3);
            UpdateDetailRow(uiRow, uiSummary);
            var apiRow = sheet1.GetRow(4);
            UpdateDetailRow(apiRow, apiSummary);
            sheet1.GetRow(1).GetCell(7).SetCellValue(runDuration);

            EvaluateFormulas(book);

            Save(saveFile, book);
        }

        public void UpdatePreviousSummaryReport(string preBuildNum)
        {
            var curBook = Read(saveFile);
            
            var preBook = Read(Path.Combine(savePath, DefaultSummaryFileName + preBuildNum + XlsxFormat));
            if (preBook == null)
                return;

            ISheet preSheet1 = preBook.GetSheetAt(0);
            var preUiRow = preSheet1.GetRow(3);
            var preApiRow = preSheet1.GetRow(4);

            ISheet curSheet1 = curBook.GetSheetAt(1);
            var curUiRow = curSheet1.GetRow(3);
            CopyDetailRow(curUiRow, preUiRow);
            var curApiRow = curSheet1.GetRow(4);
            CopyDetailRow(curApiRow, preApiRow);

            EvaluateFormulas(curBook);

            Save(saveFile, curBook);
        }


        #region Steps

        private void UpdateDetailRow(IRow row, RunSummary runSummary)
        {
            row.UpdateCell(2, runSummary.Passed);
            row.UpdateCell(3, runSummary.PassedOnRerun);
            row.UpdateCell(4, runSummary.NotExecuted);
            row.UpdateCell(5, runSummary.Failed);
        }

        private void CopyDetailRow(IRow rowTo, IRow rowFrom)
        {
            rowTo.UpdateCell(2, Convert.ToDouble(rowFrom.GetCellValue(2)));
            rowTo.UpdateCell(3, Convert.ToDouble(rowFrom.GetCellValue(3)));
            rowTo.UpdateCell(4, Convert.ToDouble(rowFrom.GetCellValue(4)));
            rowTo.UpdateCell(5, Convert.ToDouble(rowFrom.GetCellValue(5)));
        }

        #endregion Steps
    }
}
