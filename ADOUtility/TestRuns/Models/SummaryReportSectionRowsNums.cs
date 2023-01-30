namespace TestRuns.Models
{
    public class SummaryReportSectionRowsNums
    {
        public int SumRowNum { get; set; }
        public int RunRowsFirstNum => SumRowNum + 1;
        public int RunRowsLastNum { get; set; }

    }
}
