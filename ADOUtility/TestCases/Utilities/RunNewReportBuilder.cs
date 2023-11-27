using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SharedCore.FileUtilities.Excel;

namespace TestCases.Utilities
{
    public class RunNewReportBuilder : ExcelWorker
    {
        public RunNewReportBuilder(string pathToSave, string fileName)
        {
            saveFile = Path.Combine(pathToSave, DfltFileName + fileName + XlsxFormat);
            Book = new XSSFWorkbook();
            DfltFileName = "Summary_";
        }

        public IWorkbook Book { get; private set; }

        private const string XlsxFormat = ".xlsx";
        public string DfltFileName { get; set; }
        private readonly string saveFile;

        public void SaveReport()
        {

            EvaluateFormulas(Book);
            Save(saveFile, Book);
        }
    }
}
