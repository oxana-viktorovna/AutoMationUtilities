using NPOI.SS.UserModel;
using SharedCore.FileUtilities.Excel;
using TestRuns.Models;

namespace TestRuns.Utilities
{
    public class RunNewSummaryBuilder : ExcelWorker
    {
        public RunNewSummaryBuilder(IWorkbook book)
        {
            this.book = book;
            stylesBuilder = new ExcelStylesCreater(book);
        }

        private ISheet sumSheet;
        private readonly IWorkbook book;
        private readonly ExcelStylesCreater stylesBuilder;

        public void CreateNewSummaryReport(string runDurection)
        {
            sumSheet = book.CreateSheet("Summary");
            CreateHeaders(runDurection);
        }

        public void CreateUiSummary((int, RunSummary) originalRun, List<(int,RunSummary)> reruns)
        {
            var runs = new List<(int, RunSummary)>
            {
                originalRun
            };
            runs.AddRange(reruns);
            CreateTypeSummary(4, "UI", runs);
        }

        public void CreateApiSummary(int uiRunsNum, (int, RunSummary) originalRun, List<(int, RunSummary)> reruns)
        {
            var runs = new List<(int, RunSummary)>();
            runs.Add(originalRun);
            runs.AddRange(reruns);
            CreateTypeSummary(GetApiTotalRowNum(uiRunsNum), "API", runs);
        }

        private void CreateTypeSummary(int totalsRowNum, string testType, List<(int buildId, RunSummary summary)> runs)
        {
            var nonEmpltyRowsCount = 0;
            var rowNum = totalsRowNum;
            bool isCreated = true;
            for (int i = 0; i < runs.Count; i++)
            {
                var isOrig = i == 0;
                rowNum = isCreated ? rowNum + 1 : rowNum;
                isCreated = CreateRunTotals(runs[i], rowNum, isOrig);
                nonEmpltyRowsCount = isCreated ? nonEmpltyRowsCount + 1 : nonEmpltyRowsCount;
            }

            CreateLocalRunTotals(nonEmpltyRowsCount + totalsRowNum + 1);
            CreateTypeTotals(totalsRowNum, nonEmpltyRowsCount, testType);
        }

        private int GetApiTotalRowNum(int uiRunsNum)
            => 5 + 1 + uiRunsNum;

        private void CreateHeaders(string runDurection)
        {
            var headerRow = sumSheet.CreateRow(0);
            var style = stylesBuilder.GetHeaderStyle();
            headerRow.CreateCell(0, "", style);
            headerRow.CreateCell(1, $"Run duration: {runDurection}", style);
            headerRow.CreateCell(2, "Total Tests", style);
            headerRow.CreateCell(3, "Passed", style);
            headerRow.CreateCell(4, "Not Executed", style);
            headerRow.CreateCell(5, "Failed", style);
            headerRow.CreateCell(6, "Pass percentage", style);
        }

        public void CreateMainTotals(int uiRunsNum)
        {
            var rowNum = 1;
            var firstRow = sumSheet.CreateRow(rowNum);
            var uiTotalRow = 4;
            var apiTotalRow = GetApiTotalRowNum(uiRunsNum);

            var sideBarStyle = stylesBuilder.GetSideBarStyle();
            firstRow.CreateCell(0, "Totals", sideBarStyle);

            var totalTestsMainStyle = stylesBuilder.GetTotalTestsMainStyle();
            firstRow.CreateCellWithFormula(2, $"SUM(D{GetActualRowNum(rowNum)}:F{GetActualRowNum(rowNum)})", totalTestsMainStyle);

            var totalPassedMainStyle = stylesBuilder.GetTotalPassedMainStyle();
            firstRow.CreateCellWithFormula(3, $"D{GetActualRowNum(uiTotalRow)}+D{GetActualRowNum(apiTotalRow)}", totalPassedMainStyle);

            var totalNotExecMainStyle = stylesBuilder.GetTotalNotExecMainStyle();
            firstRow.CreateCellWithFormula(4, $"E{GetActualRowNum(uiTotalRow)}+E{GetActualRowNum(apiTotalRow)}", totalNotExecMainStyle);

            var totalFailedMainStyle = stylesBuilder.GetTotalFailedMainStyle();
            firstRow.CreateCellWithFormula(5, $"F{GetActualRowNum(uiTotalRow)}+F{GetActualRowNum(apiTotalRow)}", totalFailedMainStyle);

            var totalProcentMainStyle = stylesBuilder.GetTotalProcentMainStyle();
            firstRow.CreateCellWithFormula(6, $"D{GetActualRowNum(rowNum)}/C{GetActualRowNum(rowNum)}", totalProcentMainStyle);

            CreateRerunPassedTotals(uiRunsNum);

            Autosize(sumSheet);
        }

        private void CreateRerunPassedTotals(int uiRunsNum)
        { 
            var rowNum = 2;
            var row = sumSheet.CreateRow(rowNum);
            var totalPassedStyle = stylesBuilder.GetTotalReRunPassedStyle();
            var actualApiTotalRowNum = GetActualRowNum(GetApiTotalRowNum(uiRunsNum));
            row.CreateCellWithFormula(3, $"\"on re-run \"&(SUM(D7:D{sumSheet.LastRowNum}) -SUM(D{actualApiTotalRowNum}:D{actualApiTotalRowNum+1}))", totalPassedStyle);

        }

        private void CreateTypeTotals(int totalrowNum, int numOfReRuns, string testType)
        {
            var nightlyRow = totalrowNum + 1;
            var row = sumSheet.CreateRow(totalrowNum);

            var sideBarStyle = stylesBuilder.GetSideBarWithForeGroundStyle();
            row.CreateCell(0, testType, sideBarStyle);

            var totalTestsStyle = stylesBuilder.GetTotalTestsStyle();
            row.CreateCell(1, "", totalTestsStyle);

            var totalTestsMainStyle = stylesBuilder.GetTotalTestsStyle();
            row.CreateCellWithFormula(2, $"SUM(D{GetActualRowNum(totalrowNum)}:F{GetActualRowNum(totalrowNum)})", totalTestsMainStyle);

            var totalPassedMainStyle = stylesBuilder.GetTotalPassedStyle();
            row.CreateCellWithFormula(3, $"SUM(D{GetActualRowNum(nightlyRow)}:D{GetActualRowNum(nightlyRow + numOfReRuns)})", totalPassedMainStyle);

            var totalNotExecMainStyle = stylesBuilder.GetTotalNotExecStyle();
            row.CreateCellWithFormula(4, $"E{GetActualRowNum(nightlyRow)}", totalNotExecMainStyle);

            var totalFailedMainStyle = stylesBuilder.GetTotalFailedStyle();
            row.CreateCellWithFormula(5, $"F{GetActualRowNum(nightlyRow)} - E{GetActualRowNum(nightlyRow + 1)} - SUM(D{GetActualRowNum(nightlyRow + 1)}:D{GetActualRowNum(nightlyRow) + numOfReRuns})", totalFailedMainStyle);

            var totalProcentMainStyle = stylesBuilder.GetTotalProcentStyle();
            row.CreateCellWithFormula(6, $"if(D{GetActualRowNum(totalrowNum)}=0,0,D{GetActualRowNum(totalrowNum)}/C{GetActualRowNum(totalrowNum)})", totalProcentMainStyle);
        }

        private bool CreateRunTotals((int buildId, RunSummary summary) run, int rowNum, bool isOrig = false)
        {
            if (run.summary.Passed + run.summary.NotExecuted + run.summary.Failed == 0)
                return false;

            var row = sumSheet.CreateRow(rowNum);
            var title = isOrig ? "Run" : "Re-Run";
            row.CreateCell(1, title);
            row.AddHyperLink(1, $"https://dev.azure.com/tr-corp-legal-tracker/Tracker/_build/results?buildId={run.buildId}&view=ms.vss-test-web.build-test-results-tab", stylesBuilder.GetHyperlinkStyle());
            row.CreateCellWithFormula(2, $"D{GetActualRowNum(rowNum)}+E{GetActualRowNum(rowNum)}+F{GetActualRowNum(rowNum)}");
            row.CreateCell(3, run.summary.Passed);
            row.CreateCell(4, run.summary.NotExecuted);
            row.CreateCell(5, run.summary.Failed);
            var procentStyle = stylesBuilder.GetProcentStyle();
            row.CreateCellWithFormula(6, $"D{GetActualRowNum(rowNum)}/C{GetActualRowNum(rowNum)}", procentStyle);

            return true;
        }

        private void CreateLocalRunTotals(int rowNum)
        {
            var firstRow = sumSheet.CreateRow(rowNum);

            firstRow.CreateCell(1, $"Re-run Local");
            firstRow.CreateCellWithFormula(2, $"D{GetActualRowNum(rowNum)}+E{GetActualRowNum(rowNum)}+F{GetActualRowNum(rowNum)}");
            var procentStyle = stylesBuilder.GetProcentStyle();
            firstRow.CreateCellWithFormula(6, $"if(D{GetActualRowNum(rowNum)}=0,0,D{GetActualRowNum(rowNum)}/C{GetActualRowNum(rowNum)})", procentStyle);
        }
    }
}
