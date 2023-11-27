using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SharedCore.FileUtilities.Excel;
using System.Drawing;

namespace TestCases.Utilities
{
    public class ExcelStylesCreater
    {
        public ExcelStylesCreater(IWorkbook book)
        {
            this.book = book;
            dfltFontName = "Segoe UI";
        }

        private IWorkbook book;
        private readonly string dfltFontName;

        public XSSFCellStyle GetHeaderBottomBorderStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(10, dfltFontName)
                .AddAlignmrnt(VerticalAlignment.Top, HorizontalAlignment.Center)
                .AddForegroundColor(Color.LightGray, tint: 0.5)
                .AddBorder(bottom: BorderStyle.Thin)
                .Build();

            return style;
        }

        public XSSFCellStyle GetRegularBottomBorderStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(10, dfltFontName)
                .AddBorder(bottom: BorderStyle.Thin)
                .Build();

            return style;
        }
    }
}
