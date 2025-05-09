using System.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.Service;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using Shouldly;
using Xunit;

namespace GpConnect.AppointmentChecker.Api.Tests;

public class Tests
{
    private FakeLogger<ReportingService> _mockLogger;
    private IMessageService _mockMessageService;
    private IDataService _mockDataServivce;
    private IInteractionService _mockInteractionService;
    private IWorkflowService _mockWorkFlowService;
    private readonly ReportingService _reportGenerator;

    [Fact]
    public void CreateReport_ShouldGenerateGuidanceSheet_WhenNoData()
    {
        // Arrange
        var result = new DataTable();

        // Act
        using var stream = _reportGenerator.CreateReport(result);
        stream.Seek(0, SeekOrigin.Begin);
        using var doc = SpreadsheetDocument.Open(stream, false);

        var workbookPart = doc.WorkbookPart;
        var sheets = workbookPart?.Workbook.Descendants<Sheet>().ToList();

        // Assert - Sheet exists
        if (sheets == null)
        {
            Assert.Fail("No sheets found");
        }

        var guidanceSheet = sheets.FirstOrDefault(s => s.Name == "Guidance");
        guidanceSheet.ShouldNotBeNull();

        // Get the worksheet part
        var worksheetPart = (WorksheetPart)workbookPart?.GetPartById(guidanceSheet.Id);
        var rows = worksheetPart?.Worksheet.Descendants<Row>().ToList();

        // Assert - Header Row
        var headerCells = rows?[0].Elements<Cell>().ToList() ?? [];
        headerCells[0].InnerText.ShouldBe("Column");
        headerCells[1].InnerText.ShouldBe("Heading");
        headerCells[2].InnerText.ShouldBe("Description");
        headerCells[3].InnerText.ShouldBe("Example Data");

        // Assert - First Data Row
        var firstDataRowCells = rows?[1].Elements<Cell>().ToList() ?? [];
        GetCellValue(firstDataRowCells[0], workbookPart).ShouldBe("A");
        GetCellValue(firstDataRowCells[1], workbookPart).ShouldBe("ODS Code");
        GetCellValue(firstDataRowCells[2], workbookPart).ShouldContain("unique code");
        GetCellValue(firstDataRowCells[3], workbookPart).ShouldBe("A81021");
    }

    [Fact]
    public void CreateReport_ShouldGenerateGuidanceSheet_WhenData()
    {
        // Arrange
        var dataTable = new DataTable();
        dataTable.Columns.Add("ODS Code");
        dataTable.Columns.Add("Supplier Name");
        dataTable.Columns.Add("Site Name");
        dataTable.Columns.Add("Post-code");
        dataTable.Columns.Add("ICB");
        dataTable.Columns.Add("Higher Health Authority");
        dataTable.Columns.Add("Commissioning Region");
        dataTable.Columns.Add("Version");
        dataTable.Columns.Add("Allergies");

        var testData = new[]
        {
            new CapabilityData("A81021", "EGTON MEDICAL INFORMATION SYSTEMS LTD (EMIS)", "NORMANBY MEDICAL CENTRE",
                "TS6 6TD", "NHS NORTH EAST AND NORTH CUMBRIA ICB - 16C (16C)",
                "NHS NORTH EAST AND NORTH CUMBRIA INTEGRATED CARE BOARD (QHM)",
                "NORTH EAST AND YORKSHIRE COMMISSIONING REGION (Y63)", "v0.7.2", "Active"
            ),
            new CapabilityData("A81035", "EGTON MEDICAL INFORMATION SYSTEMS LTD (EMIS)", "NEWLANDS MEDICAL CENTRE",
                "TS1 3RX", "NHS NORTH EAST AND NORTH CUMBRIA ICB - 16C (16C)",
                "NHS NORTH EAST AND NORTH CUMBRIA INTEGRATED CARE BOARD (QHM)",
                "NORTH EAST AND YORKSHIRE COMMISSIONING REGION (Y63)", "v0.7.3", "Active"
            ),
            new CapabilityData("A81040", "EGTON MEDICAL INFORMATION SYSTEMS LTD (EMIS)", "MARSH HOUSE MEDICAL PRACTICE",
                "TS23 2DG", "NHS NORTH EAST AND NORTH CUMBRIA ICB - 16C (16C)",
                "NHS NORTH EAST AND NORTH CUMBRIA INTEGRATED CARE BOARD (QHM)",
                "NORTH EAST AND YORKSHIRE COMMISSIONING REGION (Y63)", "v0.7.3", "Active"
            )
        };

        foreach (var item in testData)
        {
            dataTable.Rows.Add(
                item.ODSCode,
                item.SupplierName,
                item.SiteName,
                item.Postcode,
                item.ICB,
                item.HigherHealthAuthority,
                item.CommissioningRegion,
                item.Version,
                item.Status
            );
        }

        // Act
        using var stream = _reportGenerator.CreateReport(dataTable, "Capability Report", [
            new ReportFilterRequest()
            {
                FilterColumn = "ODS Code",
                FilterTab = "Capability Report Test",
                FilterValue = "A81021"
            }
        ]);

        // Assert
        stream.Seek(0, SeekOrigin.Begin);
        using var doc = SpreadsheetDocument.Open(stream, false);

        var workbookPart = doc.WorkbookPart;
        var sheets = workbookPart?.Workbook.Descendants<Sheet>().ToList();
        sheets.ShouldNotBeNull();
        sheets.ShouldContain(s => s.Name == "Guidance");
        sheets.ShouldContain(s => s.Name == "Capability_Report_Test");

        var sheet = sheets.First(s => s.Name == "Capability_Report_Test");
        var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
        var rows = worksheetPart.Worksheet.Descendants<Row>().ToList();

        // First data row (after header), column A (ODS Code)
        var firstDataRow = rows[4];
        var firstCell = firstDataRow?.Elements<Cell>().First();
        var cellValue = GetCellValue(workbookPart, firstCell);

        cellValue.ShouldBe("A81021");
    }

    private static string GetCellValue(WorkbookPart workbookPart, Cell cell)
    {
        var value = cell.InnerText;

        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            var stringTable = workbookPart.SharedStringTablePart.SharedStringTable;
            return stringTable.ChildElements[int.Parse(value)].InnerText;
        }

        return value;
    }

// --- Helper Methods ----

    private string GetCellValue(Cell cell, WorkbookPart workbookPart)
    {
        var value = cell.CellValue?.InnerText;
        if (cell.DataType?.Value != CellValues.SharedString) return value ?? string.Empty;

        var sst = workbookPart?.SharedStringTablePart?.SharedStringTable ?? throw new NullReferenceException();
        return sst.ElementAt(int.Parse(value)).InnerText;
    }

    private record CapabilityData(
        string ODSCode,
        string SupplierName,
        string SiteName,
        string Postcode,
        string ICB,
        string HigherHealthAuthority,
        string CommissioningRegion,
        string Version,
        string Status);

    public Tests()
    {
        _mockLogger = new FakeLogger<ReportingService>();
        _mockMessageService = Substitute.For<IMessageService>();
        _mockDataServivce = Substitute.For<IDataService>();
        _mockInteractionService = Substitute.For<IInteractionService>();
        _mockWorkFlowService = Substitute.For<IWorkflowService>();

        _reportGenerator = new ReportingService(
            _mockLogger, _mockMessageService,
            _mockDataServivce, _mockInteractionService,
            _mockWorkFlowService);
    }
}