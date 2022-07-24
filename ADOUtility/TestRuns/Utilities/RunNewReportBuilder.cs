using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SharedCore.FileUtilities.Excel;

namespace TestRuns.Utilities
{
    public class RunNewReportBuilder : ExcelWorker
    {
        public RunNewReportBuilder(string pathToSave, string fileName)
        {
            saveFile = Path.Combine(pathToSave, DefaultSummaryFileName + fileName + XlsxFormat);
            Book = new XSSFWorkbook();         
        }

        public IWorkbook Book { get; private set; }
        
        private const string XlsxFormat = ".xlsx";
        private const string DefaultSummaryFileName = "TestResultSummary_";
        private readonly string saveFile;

        public void SaveReport()
        {

            EvaluateFormulas(Book);
            Save(saveFile, Book);
        }
    }
}
