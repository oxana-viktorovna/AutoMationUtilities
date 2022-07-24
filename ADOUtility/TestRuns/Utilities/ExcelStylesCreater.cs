using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SharedCore.FileUtilities.Excel;
using System.Drawing;

namespace TestRuns.Utilities
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

        public XSSFCellStyle GetHyperlinkStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(11, dfltFontName, Color.Blue, fontUnderlineType: FontUnderlineType.Single).Build();

            return style;
        }

        public XSSFCellStyle GetHeaderStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(11, dfltFontName)
                .AddAlignmrnt(VerticalAlignment.Top, HorizontalAlignment.Center)
                .AddForegroundColor(Color.LightGray)
                .Build();

            return style;
        }

        public XSSFCellStyle GetHeaderFullBorderStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(11, dfltFontName)
                .AddAlignmrnt(VerticalAlignment.Top, HorizontalAlignment.Center)
                .AddForegroundColor(Color.LightGray, tint: 0.5)
                .AddBorder(BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin)
                .Build();

            return style;
        }

        public XSSFCellStyle GetRegularFullBorderStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(11, dfltFontName)
                .AddBorder(BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin)
                .Build();

            return style;
        }

        public XSSFCellStyle GetSideBarStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(14, dfltFontName)
                .AddAlignmrnt(VerticalAlignment.Center, HorizontalAlignment.Left)
                .Build();

            return style;
        }

        public XSSFCellStyle GetSideBarWithForeGroundStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(14, dfltFontName)
                .AddAlignmrnt(VerticalAlignment.Center, HorizontalAlignment.Left)
                .AddForegroundColor(Color.LightGray, tint: 0.5)
                .Build();

            return style;
        }

        public XSSFCellStyle GetTotalTestsMainStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(27, dfltFontName, true).Build();

            return style;
        }

        public XSSFCellStyle GetTotalPassedMainStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(27, dfltFontName, Color.Green, true).Build();

            return style;
        }

        public XSSFCellStyle GetTotalNotExecMainStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(27, dfltFontName, Color.Gray, true).Build();

            return style;
        }

        public XSSFCellStyle GetTotalFailedMainStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(20, dfltFontName, Color.DarkRed, true).Build();

            return style;
        }

        public XSSFCellStyle GetTotalProcentMainStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(27, dfltFontName, Color.Green, true).Build();
            style.SetDataFormat(GetProcentFormat());
            
            return style;
        }

        public XSSFCellStyle GetTotalTestsStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(14, dfltFontName)
                .AddForegroundColor(Color.LightGray, tint: 0.5)
                .Build();

            return style;
        }

        public XSSFCellStyle GetTotalPassedStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(14, dfltFontName, Color.Green)
                .AddForegroundColor(Color.LightGray, tint: 0.5)
                .Build();

            return style;
        }

        public XSSFCellStyle GetTotalReRunPassedStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(10, dfltFontName, Color.Green)
                .Build();

            return style;
        }

        public XSSFCellStyle GetTotalNotExecStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(14, dfltFontName, Color.Gray)
                .AddForegroundColor(Color.LightGray, tint: 0.5)
                .Build();

            return style;
        }

        public XSSFCellStyle GetTotalFailedStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(14, dfltFontName, Color.DarkRed)
                .AddForegroundColor(Color.LightGray, tint: 0.5)
                .Build();

            return style;
        }

        public XSSFCellStyle GetTotalProcentStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(14, dfltFontName, Color.Green)
                .AddForegroundColor(Color.LightGray, tint: 0.5)
                .Build();
            style.SetDataFormat(GetProcentFormat());

            return style;
        }

        public XSSFCellStyle GetProcentStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(11, dfltFontName)
                .Build();
            style.SetDataFormat(GetProcentFormat());

            return style;
        }

        private int GetProcentFormat()
            => book.CreateDataFormat().GetFormat("0.00%");

    }
}
