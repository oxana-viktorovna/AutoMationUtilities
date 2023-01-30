using NPOI.SS.UserModel;
using SharedCore.FileUtilities.Excel;
using TestRuns.Models;
using TestRuns.Steps;

namespace TestRuns.Utilities
{
    public class RunNewSummaryBuilderNew : ExcelWorker
    {
        public RunNewSummaryBuilderNew(IWorkbook book, BuildApiSteps buildApiSteps)
        {
            this.book = book;
            stylesBuilder = new ExcelStylesCreater(book);
            this.buildApiSteps = buildApiSteps;
        }

        private ISheet sumSheet;
        private readonly IWorkbook book;
        private readonly BuildApiSteps buildApiSteps;
        private readonly ExcelStylesCreater stylesBuilder;
        private SummaryReportSectionRowsNums uiRowsNums;
        private SummaryReportSectionRowsNums apiRowsNums;
        private SummaryReportSectionRowsNums scriptRowsNums;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="runs">isOrig means that build was not a rerun build.</param>
        /// <param name="runDurection"></param>
        public void CreateSummaryReport(IEnumerable<(TestType testType, (int id, bool isOrig) build, RunSummary runSummary)> runs, string runDurection)
        {
            GenerateRowsNumbers(runs);

            sumSheet = book.CreateSheet("Summary");
            
            CreateHeaders(runDurection);
            GenerateAllSectionsRows(runs);
            CreateMainTotals();
        }

        private void GenerateRowsNumbers(IEnumerable<(TestType testType, (int id, bool isOrig) build, RunSummary runSummary)> runs)
        {
            var uiRuns = runs.Where(run => run.testType == TestType.Ui);
            var apiRuns = runs.Where(run => run.testType == TestType.Api);
            var scriptRuns = runs.Where(run => run.testType == TestType.Script);
            GenerateRowsNumbers(uiRuns.Count(), apiRuns.Count(), scriptRuns.Count());
        }

        private void GenerateRowsNumbers(int uiRuns, int apiRuns, int scriptRuns)
        {
            uiRowsNums = new SummaryReportSectionRowsNums();
            uiRowsNums.SumRowNum = 5;
            uiRowsNums.RunRowsLastNum = uiRowsNums.SumRowNum + uiRuns + 1;

            apiRowsNums = new SummaryReportSectionRowsNums();
            apiRowsNums.SumRowNum = uiRowsNums.RunRowsLastNum + 1;
            apiRowsNums.RunRowsLastNum = apiRowsNums.SumRowNum + apiRuns;

            scriptRowsNums = new SummaryReportSectionRowsNums();
            scriptRowsNums.SumRowNum = apiRowsNums.RunRowsLastNum + 1;
            scriptRowsNums.RunRowsLastNum = scriptRowsNums.SumRowNum + scriptRuns;
        }

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

        private void CreateMainTotals()
        {
            var rowNum = 1;
            var firstRow = sumSheet.CreateRow(rowNum);

            var sideBarStyle = stylesBuilder.GetSideBarStyle();
            firstRow.CreateCell(0, "Totals", sideBarStyle);

            var totalTestsMainStyle = stylesBuilder.GetTotalTestsMainStyle();
            firstRow.CreateCellWithFormula(2, $"SUM(D{rowNum + 1}:F{rowNum + 1})", totalTestsMainStyle);

            var totalPassedMainStyle = stylesBuilder.GetTotalPassedMainStyle();
            firstRow.CreateCellWithFormula(3, $"D{uiRowsNums.SumRowNum + 1}+D{apiRowsNums.SumRowNum + 1}+D{scriptRowsNums.SumRowNum + 1}", totalPassedMainStyle);

            var totalNotExecMainStyle = stylesBuilder.GetTotalNotExecMainStyle();
            firstRow.CreateCellWithFormula(4, $"E{uiRowsNums.SumRowNum + 1}+E{apiRowsNums.SumRowNum + 1}+E{scriptRowsNums.SumRowNum + 1}", totalNotExecMainStyle);

            var totalFailedMainStyle = stylesBuilder.GetTotalFailedMainStyle();
            firstRow.CreateCellWithFormula(5, $"F{uiRowsNums.SumRowNum + 1}+F{apiRowsNums.SumRowNum + 1}+F{scriptRowsNums.SumRowNum + 1}", totalFailedMainStyle);

            var totalProcentMainStyle = stylesBuilder.GetTotalProcentMainStyle();
            firstRow.CreateCellWithFormula(6, $"D{rowNum + 1}/C{rowNum + 1}", totalProcentMainStyle);

            //CreateRerunPassedTotals(uiRunsNum);

            Autosize(sumSheet);
        }

        public void GenerateAllSectionsRows(IEnumerable<(TestType testType, (int id, bool isOrig) build, RunSummary runSummary)> runs)
        {
            var uiRuns = runs.Where(run => run.testType == TestType.Ui);
            var apiRuns = runs.Where(run => run.testType == TestType.Api);
            var scriptRuns = runs.Where(run => run.testType == TestType.Script);

            var uiRowsNums = GetRowsNums(TestType.Ui);
            CreateSectionRows(uiRuns, uiRowsNums);

            var apiRowsNums = GetRowsNums(TestType.Api);
            CreateSectionRows(apiRuns, apiRowsNums);

            var scriptRowsNums = GetRowsNums(TestType.Script);
            CreateSectionRows(scriptRuns, scriptRowsNums);
        }

        private SummaryReportSectionRowsNums GetRowsNums(TestType testType)
        {
            return testType switch
            {
                TestType.Ui => uiRowsNums,
                TestType.Api => apiRowsNums,
                TestType.Script => scriptRowsNums,
                _ => throw new NotSupportedException($"Test type {testType} is not supported"),
            };
        }

        private void CreateSectionRows(IEnumerable<(TestType testType, (int id, bool isOrig) build, RunSummary summary)> runs, SummaryReportSectionRowsNums rowNums)
        {
            var testType = runs.First().testType;
            var origRunCount = 0;

            var i = rowNums.RunRowsFirstNum;
            foreach (var run in runs)
            {
                var isCreated = CreateRunRow((run.build, run.summary), i);
                if (isCreated && run.build.isOrig)
                    origRunCount++;

                i++;
            }

            GreateSectionTotalsRow(testType, rowNums, origRunCount);

            if (testType == TestType.Ui)
                CreateLocalRunRow(rowNums.RunRowsLastNum);
        }

        private void GreateSectionTotalsRow(TestType testType, SummaryReportSectionRowsNums rowsNums, int origRunCount)
        {
            var sideBarStyle = stylesBuilder.GetSideBarWithForeGroundStyle();
            var totalTestsStyle = stylesBuilder.GetTotalTestsStyle();
            var totalTestsMainStyle = stylesBuilder.GetTotalTestsStyle();
            var totalPassedMainStyle = stylesBuilder.GetTotalPassedStyle();
            var totalNotExecMainStyle = stylesBuilder.GetTotalNotExecStyle();
            var totalFailedMainStyle = stylesBuilder.GetTotalFailedStyle();
            var totalProcentMainStyle = stylesBuilder.GetTotalProcentStyle();

            var row = sumSheet.CreateRow(rowsNums.SumRowNum);

            row.CreateCell(0, testType.ToString(), sideBarStyle);
            row.CreateCell(1, "", totalTestsStyle);
            row.CreateCellWithFormula(2, $"SUM(D{rowsNums.SumRowNum + 1}:F{rowsNums.SumRowNum + 1})", totalTestsMainStyle);
            row.CreateCellWithFormula(3, $"SUM(D{rowsNums.RunRowsFirstNum + 1}:D{rowsNums.RunRowsLastNum + 1})", totalPassedMainStyle);
            row.CreateCellWithFormula(4, $"SUM(E{rowsNums.RunRowsFirstNum + 1}:E{rowsNums.RunRowsLastNum + 1})", totalNotExecMainStyle);

            var failedFormula = rowsNums.RunRowsFirstNum == rowsNums.RunRowsLastNum
                ? $"SUM(F{rowsNums.RunRowsFirstNum + 1}:F{rowsNums.RunRowsFirstNum + origRunCount - 1 + 1})"
                : $"SUM(F{rowsNums.RunRowsFirstNum + 1}:F{rowsNums.RunRowsFirstNum + origRunCount - 1 + 1}) - SUM(D{rowsNums.RunRowsFirstNum + origRunCount + 1}:D{rowsNums.RunRowsLastNum + 1})";
            row.CreateCellWithFormula(5, failedFormula, totalFailedMainStyle);
            row.CreateCellWithFormula(6, $"if(D{rowsNums.SumRowNum + 1}=0,0,D{rowsNums.SumRowNum + 1}/C{rowsNums.SumRowNum + 1})", totalProcentMainStyle);
        }

        private bool CreateRunRow(((int id, bool isOrig) build, RunSummary summary) run, int rowNum)
        {
            if (run.summary == null || run.summary.Passed + run.summary.NotExecuted + run.summary.Failed == 0)
                return false;

            var row = sumSheet.CreateRow(rowNum);
            var title = GetRunRowTitle(run.build.id, run.build.isOrig);
            row.CreateCell(1, title);
            row.AddHyperLink(1, GetBuildLink(run.build.id), stylesBuilder.GetHyperlinkStyle());

            row.CreateCellWithFormula(2, $"D{GetActualRowNum(rowNum)}+E{GetActualRowNum(rowNum)}+F{GetActualRowNum(rowNum)}");
            row.CreateCell(3, run.summary.Passed);
            row.CreateCell(4, run.summary.NotExecuted);
            row.CreateCell(5, run.summary.Failed);
            row.CreateCellWithFormula(6, $"D{GetActualRowNum(rowNum)}/C{GetActualRowNum(rowNum)}", stylesBuilder.GetProcentStyle());

            return true;
        }

        private string GetRunRowTitle(int buildId, bool isOrig)
        {
            var env = buildApiSteps.GetBuildEnv(buildId);
            var titleRunPart = isOrig ? "Run" : "Re-Run";

            return $"{titleRunPart} {env}";
        }

        private string GetBuildLink(int buildId)
            => buildApiSteps.GetBuildLink(buildId);

        private void CreateLocalRunRow(int rowNum)
        {
            var firstRow = sumSheet.CreateRow(rowNum);

            firstRow.CreateCell(1, $"Re-run Local");
            firstRow.CreateCellWithFormula(2, $"D{GetActualRowNum(rowNum)}+E{GetActualRowNum(rowNum)}+F{GetActualRowNum(rowNum)}");
            var procentStyle = stylesBuilder.GetProcentStyle();
            firstRow.CreateCellWithFormula(6, $"if(D{GetActualRowNum(rowNum)}=0,0,D{GetActualRowNum(rowNum)}/C{GetActualRowNum(rowNum)})", procentStyle);
        }
    }

    public enum TestType
    {
        Ui,
        Api,
        Script
    }
}
