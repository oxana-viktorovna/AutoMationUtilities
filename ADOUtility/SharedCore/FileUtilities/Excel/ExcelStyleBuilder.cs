using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Drawing;

namespace SharedCore.FileUtilities.Excel
{
    public class ExcelStyleBuilder
    {
        public ExcelStyleBuilder(IWorkbook book)
        {
            this.book = book;
            style = (XSSFCellStyle)book.CreateCellStyle();
        }

        private readonly IWorkbook book;
        private XSSFCellStyle style;

        public XSSFCellStyle Build()
            => style;

        public ExcelStyleBuilder AddFont(short hight, string fontName, bool isBold = false)
        {
            var font = CreateFont(hight, fontName, isBold);
            style.SetFont(font);

            return this;
        }

        public ExcelStyleBuilder AddFont(short hight,  string fontName, Color color, bool isBold = false, FontUnderlineType? fontUnderlineType = null)
        {
            var font = CreateFont(hight, color, fontName, isBold, fontUnderlineType);
            style.SetFont(font);

            return this;
        }

        public ExcelStyleBuilder AddForegroundColor(Color color, FillPattern? fillPattern = null, double tint = 1)
        {
            var xssfColor = new XSSFColor(color);
            xssfColor.Tint = tint;
            style.SetFillForegroundColor(xssfColor);
            fillPattern ??= FillPattern.SolidForeground;
            style.FillPattern = (FillPattern)fillPattern;

            return this;
        }

        public ExcelStyleBuilder AddAlignmrnt(VerticalAlignment? vertical = null, HorizontalAlignment? horizontal = null)
        {
            vertical ??= VerticalAlignment.Center;
            style.VerticalAlignment = (VerticalAlignment)vertical;

            horizontal ??= HorizontalAlignment.Left;
            style.Alignment = (HorizontalAlignment)horizontal;

            return this;
        }

        public ExcelStyleBuilder AddBorder(BorderStyle? top = null, BorderStyle? bottom = null, BorderStyle? left= null, BorderStyle? right = null)
        {
            top ??= BorderStyle.None;
            style.BorderTop = (BorderStyle)top;
            bottom ??= BorderStyle.None;
            style.BorderBottom = (BorderStyle)bottom;
            left ??= BorderStyle.None;
            style.BorderLeft = (BorderStyle)left;
            right ??= BorderStyle.None;
            style.BorderRight = (BorderStyle)right;

            return this;
        }

        private XSSFFont CreateFont(short hight, string fontName, bool isBold = false)
        {
            XSSFFont font = (XSSFFont)book.CreateFont();
            font.FontHeightInPoints = hight;
            font.FontName = fontName;
            font.IsBold = isBold;

            return font;
        }

        private XSSFFont CreateFont(short hight, Color color, string fontName, bool isBold = false, FontUnderlineType? fontUnderlineType = null)
        {
            XSSFFont font = CreateFont(hight, fontName, isBold);
            font.SetColor(new XSSFColor(color));
            fontUnderlineType ??= FontUnderlineType.None;
            font.Underline = (FontUnderlineType)fontUnderlineType;

            return font;
        }
    }
}
