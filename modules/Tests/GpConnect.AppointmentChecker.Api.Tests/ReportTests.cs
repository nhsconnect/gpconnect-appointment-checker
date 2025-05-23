using System.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using gpconnect_appointment_checker.api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.Service;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using Shouldly;
using Xunit;

namespace GpConnect.AppointmentChecker.Api.Tests;

public class ReportTests
{
    private FakeLogger<ReportingService> _mockLogger;
    private IMessageService _mockMessageService;
    private IDataService _mockDataServivce;
    private IInteractionService _mockInteractionService;
    private IWorkflowService _mockWorkFlowService;
    private readonly ReportingService _reportGenerator;
    private DataTable _sharedDataTable;


    #region CapabilityReports

    [Fact]
    public void CreateReport_ShouldGenerateGuidanceSheet_WhenNoData_AndCapabilityReport()
    {
        // Arrange
        var result = new DataTable();

        // Act
        using var stream = _reportGenerator.CreateReport(result, "test report", reportType: ReportType.Capability);
        stream.Seek(0, SeekOrigin.Begin);
        using var doc = SpreadsheetDocument.Open(stream, false);

        var workbookPart = doc.WorkbookPart;
        var sheets = workbookPart?.Workbook.Descendants<Sheet>().ToList();

        // Assert - Sheet exists
        if (sheets == null)
        {
            Assert.Fail("No sheets found");
        }

        sheets.Count.ShouldBe(1);

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
    public void CreateReport_ShouldGenerateGuidanceSheet_WhenData_AndCapabilityReport()
    {
        // Act
        using var stream = _reportGenerator.CreateReport(_sharedDataTable, "Capability Report",
            ReportType.Capability, [
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

        var sheetNames = sheets.Select(s => s.Name).ToList();

        sheets.ShouldNotBeNull();
        sheets.Count.ShouldBe(3); // all records + 1 filter + cap cover sheet
        sheets.ShouldContain(s => s.Name == "Guidance");
        sheets.ShouldContain(s => s.Name == "Capability_Report_Test");

        var sheet = sheets.First(s => s.Name == "Capability_Report_Test");
        var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
        var rows = worksheetPart.Worksheet.Descendants<Row>().ToList();

        // First data row (after header), column A (ODS Code)
        var firstDataRow = rows[4];
        var firstCell = firstDataRow?.Elements<Cell>().First();
        var cellValue = GetCellValue(firstCell, workbookPart);

        cellValue.ShouldBe("A81021");
    }

    [Fact]
    public void CreateReport_ShouldNot_GenerateGuidanceSheet_When_NotCapabilityReport()
    {
        // Act
        using var stream = _reportGenerator.CreateReport(_sharedDataTable, "Capability Report", ReportType.Interaction,
        [
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
        sheets.ShouldNotContain(s => s.Name == "Guidance");
        sheets.ShouldContain(s => s.Name == "Capability_Report_Test");

        var sheet = sheets.First(s => s.Name == "Capability_Report_Test");
        var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
        var rows = worksheetPart.Worksheet.Descendants<Row>().ToList();

        // First data row (after header), column A (ODS Code)
        var firstDataRow = rows[4];
        var firstCell = firstDataRow?.Elements<Cell>().First();
        var cellValue = GetCellValue(firstCell, workbookPart);

        cellValue.ShouldBe("A81021");
    }

    #endregion


    [Fact]
    public void CreateReport_ShouldNot_AllowMultipleSheetsWithTheSameName()
    {
        // Act
        using var stream = _reportGenerator.CreateReport(_sharedDataTable, "Capability Report", ReportType.Interaction,
        [
            new ReportFilterRequest()
            {
                FilterColumn = "ODS Code",
                FilterTab = "Capability Report Test",
                FilterValue = "A81021"
            },
            new ReportFilterRequest()
            {
                FilterColumn = "ODS Code",
                FilterTab = "Capability Report Test",
                FilterValue = "A81035"
            },
            new ReportFilterRequest()
            {
                FilterColumn = "Supplier Name",
                FilterTab = "Capability Report Test",
                FilterValue = "EGTON MEDICAL INFORMATION SYSTEMS LTD (EMIS)"
            }
        ]);

        // Assert
        stream.Seek(0, SeekOrigin.Begin);
        using var doc = SpreadsheetDocument.Open(stream, false);

        var workbookPart = doc.WorkbookPart;
        var sheets = workbookPart?.Workbook.Descendants<Sheet>().ToList();

        sheets.ShouldNotBeNull();
        sheets.Count.ShouldBe(3);

        var sheetNames = sheets.Select(s => s.Name).ToList();
        sheetNames.ShouldContain(s => s.Value == "Capability_Report_Test");
        sheetNames.ShouldContain(s => s.Value == "Capability_Report_Test_1");
        sheetNames.ShouldContain(s => s.Value == "Capability_Report_Test_2");
    }

    [Fact]
    public void CreateReport_ShouldNot_CreateASheet_When_NoDataForFilter()
    {
        using var stream = _reportGenerator.CreateReport(_sharedDataTable, "Capability Report", ReportType.Interaction,
        [
            new ReportFilterRequest()
            {
                FilterColumn = "ODS Code",
                FilterTab = "No results to find",
                FilterValue = "BAOI819"
            },
            new ReportFilterRequest()
            {
                FilterColumn = "ODS Code",
                FilterTab = "A81035 Records",
                FilterValue = "A81035"
            }
        ]);

        // Assert
        stream.Seek(0, SeekOrigin.Begin);
        using var doc = SpreadsheetDocument.Open(stream, false);

        var workbookPart = doc.WorkbookPart;
        var sheets = workbookPart?.Workbook.Descendants<Sheet>().ToList();

        var sheetNames = sheets?.Select(s => s.Name).ToList();
        sheetNames.ShouldNotBeNull();
        sheetNames.ShouldNotBeEmpty();
        sheetNames.Count.ShouldBe(2); // all records + filter
        sheetNames.ShouldContain(s => s.Value == "A81035_Records");
        sheetNames.ShouldNotContain(s => s.Value == "No results to find");
    }
    // --- Helper Methods ----


    [Theory]
    [InlineData("", "Sheet")]
    [InlineData(null, "Sheet")]
    [InlineData("New Name", "New_Name")]
    [InlineData("NewName", "NewName")]
    public void CreateReport_Should_HandleFilterTabNamesCorrectly_WhenGivenInvalidString(string? filterName,
        string expectedName)
    {
        using var stream = _reportGenerator.CreateReport(_sharedDataTable, "Capability Report", ReportType.Interaction,
        [
            new ReportFilterRequest()
            {
                FilterColumn = "ODS Code",
                FilterTab = filterName,
                FilterValue = "A81021"
            },
        ]);

        // Assert
        stream.Seek(0, SeekOrigin.Begin);
        using var doc = SpreadsheetDocument.Open(stream, false);

        var workbookPart = doc.WorkbookPart;
        var sheets = workbookPart?.Workbook.Descendants<Sheet>().ToList();

        sheets.ShouldNotBeNull();
        sheets.Count.ShouldBe(2);
        var sheetNames = sheets.Select(s => s.Name).ToList();
        sheetNames.ShouldContain(s => s.Value == expectedName);
    }


    private static string GetCellValue(Cell cell, WorkbookPart workbookPart)
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

    public ReportTests()
    {
        // Arrange
        _sharedDataTable = new DataTable();
        _sharedDataTable.Columns.Add("ODS Code");
        _sharedDataTable.Columns.Add("Supplier Name");
        _sharedDataTable.Columns.Add("Site Name");
        _sharedDataTable.Columns.Add("Post-code");
        _sharedDataTable.Columns.Add("ICB");
        _sharedDataTable.Columns.Add("Higher Health Authority");
        _sharedDataTable.Columns.Add("Commissioning Region");
        _sharedDataTable.Columns.Add("Version");
        _sharedDataTable.Columns.Add("Allergies");

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
            _sharedDataTable.Rows.Add(
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