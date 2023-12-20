using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace GpConnect.AppointmentChecker.Api.Helpers;

public static class SpreadsheetExtensions
{
    public static EnumValue<CellValues> GetCellDataType(this string cellValue)
    {
        if (bool.TryParse(cellValue, out _))
        {
            return CellValues.Boolean;
        }
        if (int.TryParse(cellValue, out _))
        {
            return CellValues.Number;
        }
        return CellValues.String;
    }
}
