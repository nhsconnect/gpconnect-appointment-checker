using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using System.Data;

namespace GpConnect.AppointmentChecker.Api.Service;

public class ReportingService : IReportingService
{
    private readonly IDataService _dataService;

    public ReportingService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<MemoryStream> ExportByReportName(string functionName, string reportName = "")
    {
        var result = await _dataService.ExecuteFunctionAndGetDataTable($"reporting.{functionName}");
        var spreadsheetDocument = CreateReport(result, reportName);
        return spreadsheetDocument;
    }

    public async Task<MemoryStream> ExportBySpineMessage(int spineMessageId, int userId, string reportName)
    {
        var parameters = new Dictionary<string, int>
            {
                { "_user_id", userId },
                { "_spine_message_id", spineMessageId }
            };
        var result = await _dataService.ExecuteFunctionAndGetDataTable("application.get_spine_message_by_id", parameters);
        var report = CreateReport(result);
        return report;
    }

    public async Task<string> GetReport(string functionName)
    {
        var dataTable = await _dataService.ExecuteFunctionAndGetDataTable($"reporting.{functionName}", null);
        var result = dataTable.ConvertObjectToJsonData();
        return result;
    }

    public async Task<List<Report>> GetReports()
    {
        var functionName = "reporting.get_reports";
        var result = await _dataService.ExecuteQuery<Report>(functionName);
        return result;
    }

    public MemoryStream CreateReport(DataTable result, string reportName = "")
    {
        var memoryStream = new MemoryStream();
        var spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook);
        var workbookPart = spreadsheetDocument.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();
        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

        var sheetData = new SheetData();

        WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
        workbookStylesPart.Stylesheet = StyleSheetBuilder.CreateStylesheet();
        workbookStylesPart.Stylesheet.Save();

        var workSheet = new Worksheet();

        var columns = BuildColumns(result);
        workSheet.Append(columns);
        workSheet.Append(sheetData);

        worksheetPart.Worksheet = workSheet;

        var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
        var sheet = new Sheet
        {
            Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
            SheetId = 1,
            Name = reportName
        };

        sheets.AppendChild(sheet);

        BuildWorksheetHeader(sheetData, reportName);
        BuildHeaderRow(sheetData, result.Columns);
        BuildDataRows(sheetData, result.Rows);

        workbookPart.Workbook.Save();
        spreadsheetDocument.Close();

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    private Columns BuildColumns(DataTable result)
    {
        var columns = new Columns();
        for (var i = 0; i < result.Columns.Count; i++)
        {
            var maxColumnLength = result.AsEnumerable().Max(row => row.Field<object>(result.Columns[i].ColumnName)?.ToString()?.Length);
            var columnNameLength = result.Columns[i].ColumnName.Length;
            var col = new Column
            {
                CustomWidth = true,
                Width = maxColumnLength < columnNameLength ? columnNameLength * 2 : maxColumnLength.GetValueOrDefault(),
                Min = Convert.ToUInt32(i + 1),
                Max = Convert.ToUInt32(i + 1)
            };
            columns.AppendChild(col);
        }
        return columns;
    }

    private void BuildDataRows(SheetData sheetData, DataRowCollection dataRowCollection)
    {
        for (var i = 0; i < dataRowCollection.Count; i++)
        {
            var row = new Row();
            var dataRow = dataRowCollection[i];

            for (var j = 0; j < dataRow.ItemArray.Length; j++)
            {
                var cellValue = dataRow.ItemArray[j]?.ToString();
                var cell = new Cell
                {
                    DataType = cellValue.GetCellDataType(),
                    CellValue = cellValue?.Length > 0 ? new CellValue(cellValue) : null
                };
                row.AppendChild(cell);
            }
            sheetData.AppendChild(row);
        }
    }

    private void BuildHeaderRow(SheetData sheetData, DataColumnCollection dataColumnCollection)
    {
        var headerRow = new Row();
        for (var j = 0; j < dataColumnCollection.Count; j++)
        {
            var column = new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(dataColumnCollection[j].ColumnName),
                StyleIndex = 2
            };
            headerRow.AppendChild(column);
        }
        sheetData.AppendChild(headerRow);
    }

    private void BuildWorksheetHeader(SheetData sheetData, string reportName)
    {
        var row1 = new Row { Height = 55 };
        var titleCell = new Cell
        {
            DataType = CellValues.String,
            CellValue = new CellValue(reportName),
            StyleIndex = 1
        };

        row1.AppendChild(titleCell);
        sheetData.AppendChild(row1);

        var row2 = new Row { Height = 40 };
        var subTitleCell = new Cell
        {
            DataType = CellValues.String,
            CellValue = new CellValue($"Report generated on {DateTime.Now:D} at {DateTime.Now:T}"),
            StyleIndex = 3
        };
        row2.AppendChild(subTitleCell);
        sheetData.AppendChild(row2);

        var row3 = new Row { Height = 30 };
        sheetData.AppendChild(row3);
    }
}
