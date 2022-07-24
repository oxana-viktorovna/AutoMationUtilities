using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;

namespace SharedCore.FileUtilities.Excel
{
    public class ExcelWorker
    {
        protected static void Save(string path, IWorkbook book)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                book.Write(fs);
            }
        }

        protected static IWorkbook Read(string path)
        {
            IWorkbook book;

            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                try
                {
                    book = new XSSFWorkbook(fs);
                    return book;
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        protected static void EvaluateFormulas(IWorkbook workbook)
        {
            if (workbook is XSSFWorkbook)
            {
                XSSFFormulaEvaluator.EvaluateAllFormulaCells(workbook);
            }
            else
            {
                HSSFFormulaEvaluator.EvaluateAllFormulaCells(workbook);
            }
        }

        protected void Autosize(ISheet sheet)
        {
            int numberOfColumns = sheet.GetRow(1).PhysicalNumberOfCells;
            for (int i = 1; i <= numberOfColumns; i++)
            {
                sheet.AutoSizeColumn(i);
                GC.Collect();
            }
        }

        protected int GetActualRowNum(int rowNum)
            => rowNum + 1;
    }
}
