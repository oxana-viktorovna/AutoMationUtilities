using NPOI.SS.UserModel;
using SharedCore.FileUtilities.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Statistic.Utilities
{
    internal class ExcelAutoStatWorker : ExcelWorker
    {
        public ExcelAutoStatWorker(string pathToRead)
        {
            //var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //book = Read(Path.Combine(assemblyPath, @"StatisticResources\AutoUIStatistic.xlsx"));
            book = Read(pathToRead);
        }

        private IWorkbook book;

        internal void PopulateAutoStat(
            string pathToSave, 
            IEnumerable<(string priority, int autoCount, int allCount)> counts,
            DateTime? asOf = null)
        {
            var origSheetName = book.GetSheetAt(0).SheetName;
            ISheet curSheet = book.CloneSheet(0);

            UpdateDate(curSheet, asOf);
            UpdateStatisticRow(curSheet, 4, GetCountsByPriority("0", counts));
            UpdateStatisticRow(curSheet, 5, GetCountsByPriority("1", counts));
            UpdateStatisticRow(curSheet, 6, GetCountsByPriority("2+", counts));

            for (int rowNum = 2; rowNum < 7; rowNum++)
            {
                UpdateFormula(curSheet, rowNum, 2,  origSheetName);
            }

            var curSheetIndex = book.GetSheetIndex(curSheet.SheetName);
            var newCurSheetName = AsOfToTitle(asOf);
            book.SetSheetName(curSheetIndex, newCurSheetName);
            book.SetSheetOrder(newCurSheetName, 0);
            EvaluateFormulas(book);

            Save(pathToSave, book);
        }

        private void UpdateDate(ISheet sheet, DateTime? asOf)
        {
            sheet.GetRow(0).GetCell(1).SetCellValue(AsOfToTitle(asOf));
        }

        private string AsOfToTitle(DateTime? asOf)
        {
            var date = asOf ?? DateTime.Now;

            return $"{date:M}";
        }

        private void UpdateStatisticRow(ISheet sheet, int rowNum, (int autoCount, int allCount) counts)
        {
            var row = sheet.GetRow(rowNum);
            row.GetCell(3).SetCellValue(counts.autoCount);
            row.GetCell(4).SetCellValue(counts.allCount);
        }

        private void UpdateFormula(ISheet curSheet, int rowNum, int cellNum, string origSheetName)
        {
            var cell = curSheet.GetRow(rowNum).GetCell(cellNum);
            var cellFormula = cell.CellFormula;
            var regex = new Regex(@"-(\w*|'\w*')!");
            var match = regex.Match(cellFormula).Groups[1].Value;
            var newFormula = cellFormula.Replace(match, origSheetName.Contains(' ')? $"'{origSheetName}'": origSheetName);
            cell.SetCellFormula(newFormula);
        }

        private (int autoCount, int allCount) GetCountsByPriority(string targetPriority, IEnumerable<(string priority, int autoCount, int allCount)> counts)
            => counts.Where(c => c.priority.Equals(targetPriority, StringComparison.OrdinalIgnoreCase))
            .Select(c => (c.autoCount, c.allCount))
            .FirstOrDefault();
    }
}
