using NPOI.SS.UserModel;
using SharedCore.FileUtilities.Excel;
using System;
using System.IO;
using System.Reflection;
using TestRuns.Models;

namespace TestRuns.Utilities
{
    internal class RunSummaryWorker : ExcelWorker
    {
        private const string XlsxFormat = ".xlsx";
        private const string DefaultSummaryFileName = "TestResultSummary_";
        public void UpdateCurrentSummaryReport(string pathToSave, string fileName, RunSummary uiSummary, RunSummary apiSummary, string runDuration)
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

            var path = Path.Combine(pathToSave, DefaultSummaryFileName + fileName + XlsxFormat);
            Save(path, book);
        }

        public void UpdatePreviousSummaryReport(string path, string currBuildNum, string preBuildNum)
        {
            var curBookPath = Path.Combine(path, DefaultSummaryFileName + currBuildNum + XlsxFormat);
            var curBook = Read(curBookPath);
            
            var preBook = Read(Path.Combine(path, DefaultSummaryFileName + preBuildNum + XlsxFormat));
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

            Save(curBookPath, curBook);
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
