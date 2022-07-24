using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace SharedCore.FileUtilities.Excel
{
    public static class RowExtension
    {
        public static void UpdateCell(this IRow row, int column, double value)
        {
            var cell = row.GetCell(column);
            cell.SetCellValue(value);
        }

        public static string GetCellValue(this IRow row, int column)
        {
            var cell = row.GetCell(column);

            return cell.CellType switch
            {
                CellType.String => cell.StringCellValue,
                CellType.Numeric => cell.NumericCellValue.ToString(),
                _ => null,
            };
        }

        public static void CreateCell(this IRow row, int column, string value, ICellStyle style = null)
        {
            ICell cell = row.CreateCell(column);
            cell.SetCellValue(value);

            if(style!=null)
                cell.CellStyle = style;
        }

        public static void CreateCell(this IRow row, int column, int value, ICellStyle style = null)
        {
            ICell cell = row.CreateCell(column);
            cell.SetCellValue(value);

            if (style != null)
                cell.CellStyle = style;
        }

        public static void CreateCellWithFormula(this IRow row, int column, string formula, ICellStyle style = null)
        {
            var cell = row.CreateCell(column);
            cell.CellFormula = formula;

            if (style != null)
                cell.CellStyle = style;
        }

        public static void AddHyperLink(this IRow row, int column, string url, ICellStyle style = null)
        {
            var link = new XSSFHyperlink(HyperlinkType.Url)
            {
                Address = (url)
            };
            var cell = row.GetCell(column);
            cell.Hyperlink = link;

            if (style != null)
                cell.CellStyle = style;
        }
    }
}
