using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace C4S.Services.Services.ExcelWorksheetService.Extensions
{
    /// <inheritdoc cref="ExcelRange"/>
    public static class ExcelRangeExtensions
    {
        /// <summary>
        /// Устанавливает указанному <paramref name="range"/> <paramref name="value"/>, <paramref name="horizontalAlignment"/> и <paramref name="verticalAlignment"/>
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="horizontalAlignment"><inheritdoc cref="ExcelHorizontalAlignment"/></param>
        /// <param name="verticalAlignment"><inheritdoc cref="ExcelVerticalAlignment"/></param>
        public static void SetCellValueWithAligment(this ExcelRange? range,
            object? value,
            ExcelHorizontalAlignment horizontalAlignment = default,
            ExcelVerticalAlignment verticalAlignment = default)
        {
            if (range is not null)
            {
                range.Value = value;
                range.Style.HorizontalAlignment = horizontalAlignment;
                range.Style.VerticalAlignment = verticalAlignment;
            }
        }
    }
}