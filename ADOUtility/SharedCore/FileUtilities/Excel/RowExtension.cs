using NPOI.SS.UserModel;

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
    }
}
