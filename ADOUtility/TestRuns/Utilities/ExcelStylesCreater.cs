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

        public XSSFCellStyle GetSimpleWrapStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(10, dfltFontName).Build();

            return style;
        }

        public XSSFCellStyle GetHeaderStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(11, dfltFontName)
                .AddAlignment(VerticalAlignment.Top, HorizontalAlignment.Center)
                .AddForegroundColor(Color.Gray, tint:0.5)
                .Build();

            return style;
        }

        public XSSFCellStyle GetHeaderBottomBorderStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(10, dfltFontName)
                .AddAlignment(VerticalAlignment.Top, HorizontalAlignment.Center)
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

        public XSSFCellStyle GetSideBarStyle()
        {
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(14, dfltFontName)
                .AddAlignment(VerticalAlignment.Center, HorizontalAlignment.Left)
                .AddForegroundColor(Color.Gray, tint:0.5)
                .Build();

            return style;
        }

        public XSSFCellStyle GetSideBarWithForeGroundStyle(string type = null)
        {
            var builder = new ExcelStyleBuilder(book);
            var fontStyle = GetStyleByTestType (type);
            short size = 14;

            if (type == "Script")
                size = 12;

            var style = builder.AddFont(size, dfltFontName)
                .AddAlignment(VerticalAlignment.Center, HorizontalAlignment.Left)
                .AddForegroundColor(fontStyle.color, tint: fontStyle._tint)
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

        public XSSFCellStyle GetTotalTestsStyle(string type = null)
        {
            var fontStyle = GetStyleByTestType(type);
            var builder = new ExcelStyleBuilder(book);
            var style = builder.AddFont(14, dfltFontName)
                .AddForegroundColor(fontStyle.color, tint: fontStyle._tint)
                .Build();

            return style;
        }

        public XSSFCellStyle GetTotalPassedStyle(string type = null)
        {
            var builder = new ExcelStyleBuilder(book);
            var fontStyle = GetStyleByTestType(type);
            var style = builder.AddFont(14, dfltFontName, Color.Green)
                .AddForegroundColor(fontStyle.color, tint: fontStyle._tint)
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

        public XSSFCellStyle GetTotalNotExecStyle(string type = null)
        {
            var builder = new ExcelStyleBuilder(book);
            var fontStyle = GetStyleByTestType(type);
            var style = builder.AddFont(14, dfltFontName, Color.Gray)
                .AddForegroundColor(fontStyle.color, tint: fontStyle._tint)
                .Build();

            return style;
        }

        public XSSFCellStyle GetTotalFailedStyle(string type = null)
        {
            var builder = new ExcelStyleBuilder(book);
            var fontStyle = GetStyleByTestType(type);
            var style = builder.AddFont(14, dfltFontName, Color.DarkRed)
                .AddForegroundColor(fontStyle.color, tint: fontStyle._tint)
                .Build();

            return style;
        }

        public XSSFCellStyle GetTotalProcentStyle(string type = null)
        {
            var builder = new ExcelStyleBuilder(book);
            var fontStyle = GetStyleByTestType(type);
            var style = builder.AddFont(14, dfltFontName, Color.Green)
                .AddForegroundColor(fontStyle.color, tint: fontStyle._tint)
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

        public (Color color, double _tint) GetStyleByTestType(string type)
        {
            Color color = Color.LightGray;
            double _tint = 0.5;

            if (type != null)
            {
                switch (type)
                {
                    case "UI":
                        color = Color.LightGray;
                        _tint = 0;
                        break;
                    case "API":
                        color = Color.Gray;
                        _tint = 0.8;
                        break;
                    case "Script":
                        color = Color.LightGray;
                        _tint = 0.8;
                        break;
                }
            }

            return (color, _tint);
        }

        private int GetProcentFormat()
            => book.CreateDataFormat().GetFormat("0.00%");

    }
}
