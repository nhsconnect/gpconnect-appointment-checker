using Amazon.SQS.Model;
using Dapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using gpconnect_appointment_checker.api.DTO.Response.Reporting;
using Newtonsoft.Json;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Security.Policy;
using gpconnect_appointment_checker.api.DTO.Request;

namespace GpConnect.AppointmentChecker.Api.Service;

public class ReportingService : IReportingService
{
    private readonly ILogger<ReportingService> _logger;
    private readonly IDataService _dataService;
    private readonly IMessageService _messageService;
    private readonly IWorkflowService _workflowService;
    private readonly IInteractionService _interactionService;
    private const string CoverSheetName = "Guidance";

    public ReportingService(ILogger<ReportingService> logger, IMessageService messageService, IDataService dataService,
        IInteractionService interactionService, IWorkflowService workflowService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _workflowService = workflowService ?? throw new ArgumentNullException(nameof(workflowService));
        _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
    }

    public async Task SendMessageToCreateInteractionReportContent(ReportInteractionRequest reportInteractionRequest)
    {
        var request = JsonConvert.SerializeObject(reportInteractionRequest, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        });

        await _messageService.SendMessageToQueue(new SendMessageRequest()
        {
            MessageGroupId = reportInteractionRequest.MessageGroupId.ToString(),
            MessageBody = request
        });
    }

    public async Task<Stream> ExportReport(ReportRequest reportRequest)
    {
        var dataTable =
            await _dataService.ExecuteFunctionAndGetDataTable($"reporting.{reportRequest.FunctionName}", null);

        var memoryStream = CreateReport(dataTable, reportRequest.ReportName, reportType: ReportType.Interaction);
        return memoryStream;
    }

    public async Task<Stream> CreateInteractionReport(ReportCreationRequest reportCreationRequest)
    {
        const string functionName = "reporting.get_transient_data";
        var parameters = new DynamicParameters();
        parameters.Add("_transient_report_id", reportCreationRequest.ReportId, DbType.String, ParameterDirection.Input);
        var response = await _dataService.ExecuteQueryFirstOrDefault<TransientData>(functionName, parameters);
        var dataTable = response?.Data?.ConvertJsonDataToDataTable("ODS_Code");
        return CreateReport(dataTable, reportCreationRequest.ReportName, ReportType.Interaction,
            reportCreationRequest.ReportFilter);
    }

    public async Task RouteReportRequest(RouteReportRequest routeReportRequest)
    {
        try
        {
            string? transientData = null;
            switch (routeReportRequest.ReportId.ToLower())
            {
                case "accessrecordstructured":
                    transientData =
                        await _interactionService.CreateInteractionData<AccessRecordStructuredReporting>(
                            routeReportRequest);
                    break;
                case "appointmentmanagement":
                    transientData =
                        await _interactionService.CreateInteractionData<AppointmentManagementReporting>(
                            routeReportRequest);
                    break;
                case "accessrecordhtml":
                    transientData =
                        await _interactionService.CreateInteractionData<AccessRecordHtmlReporting>(routeReportRequest);
                    break;
                case "updaterecord":
                case "senddocument":
                    transientData = await _workflowService.CreateWorkflowData<MailboxReporting>(routeReportRequest);
                    break;
                default:
                    break;
            }

            if (transientData != null)
            {
                await CreateTransientData(transientData, routeReportRequest);
            }
        }
        catch (Exception exc)
        {
            _logger?.LogError(exc,
                "An error has occurred while attempting to execute the function 'CreateInteractionData'");
            throw;
        }
    }

    private async Task CreateTransientData(string transientData, RouteReportRequest routeReportRequest)
    {
        if (transientData != null)
        {
            var functionName = "reporting.add_transient_data";
            var parameters = new DynamicParameters();
            parameters.Add("_transient_id", routeReportRequest.ObjectKeyJson, DbType.String, ParameterDirection.Input);
            parameters.Add("_transient_data", transientData, DbType.String, ParameterDirection.Input);
            parameters.Add("_transient_report_id", routeReportRequest.ReportId, DbType.String,
                ParameterDirection.Input);
            parameters.Add("_transient_report_name", routeReportRequest.ReportName, DbType.String,
                ParameterDirection.Input);
            await _dataService.ExecuteQuery(functionName, parameters);
        }
    }

    public async Task<MemoryStream> ExportBySpineMessage(int spineMessageId, string reportName)
    {
        var parameters = new Dictionary<string, int>
        {
            { "_user_id", LoggingHelper.GetIntegerValue(Helpers.Constants.Headers.UserId) },
            { "_spine_message_id", spineMessageId }
        };
        var result =
            await _dataService.ExecuteFunctionAndGetDataTable("application.get_spine_message_by_id", parameters);
        return CreateReport(result);
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

    public async Task<List<CapabilityReport>> GetCapabilityReports()
    {
        var functionName = "reporting.get_capability_reports";
        var result = await _dataService.ExecuteQuery<CapabilityReport>(functionName);
        return result;
    }

    public MemoryStream CreateReport(DataTable? result, string reportName = "",
        ReportType reportType = ReportType.SlotSummary,
        List<ReportFilterRequest>? reportFilterRequest = null)
    {
        try
        {
            var memoryStream = new MemoryStream();
            if (result == null) return memoryStream;

            reportFilterRequest = reportFilterRequest?.OrderBy(x => x.FilterValue).ToList();

            using (var spreadsheetDocument =
                   SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = spreadsheetDocument.WorkbookPart ?? spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                workbookStylesPart.Stylesheet = StyleSheetBuilder.CreateStylesheet();
                workbookStylesPart.Stylesheet.Save();

                HashSet<string> usedSheetNames = [];
                uint nextSheetId = 1;

                // create cover sheet
                if (reportType == ReportType.Capability)
                {
                    CreateDataDictionarySheet(spreadsheetDocument, CoverSheetName, nextSheetId++);
                    usedSheetNames.Add(CoverSheetName);
                }

                if (reportFilterRequest != null)
                {
                    foreach (var request in reportFilterRequest)
                    {
                        var filteredList = result.AsEnumerable().Where(r =>
                            r.Field<string>(request.FilterColumn) ==
                            request.FilterValue);

                        if (!filteredList.Any()) continue;

                        CreateSheet(filteredList.CopyToDataTable(), reportName, spreadsheetDocument,
                            nextSheetId++,
                            usedSheetNames,
                            request.FilterValue,
                            request.FilterTab
                        );

                        var toDelete = new List<DataRow>();
                        toDelete.AddRange(filteredList.AsEnumerable());
                        toDelete.ForEach(dr => result.Rows.Remove(dr));
                    }

                    CreateSheet(
                        result,
                        reportName,
                        spreadsheetDocument,
                        nextSheetId,
                        usedSheetNames: usedSheetNames);
                }
                else
                {
                    CreateSheet(result, reportName, spreadsheetDocument, nextSheetId, usedSheetNames);
                }

                spreadsheetDocument.WorkbookPart?.Workbook.Save();
            }

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Messed up");
            throw;
        }
    }

    private static void CreateSheet(
        DataTable result,
        string reportName,
        SpreadsheetDocument spreadsheetDocument,
        uint sheetId,
        HashSet<string> usedSheetNames,
        string? filterValue = null,
        string? filterTab = null)
    {
        var baseName = filterTab?.ReplaceNonAlphanumeric() ?? "Sheet";
        var sheetName = baseName;
        var suffix = 1;
        const int sheetNameCharLimit = 31;

        // excel has char limit of 31 for sheet names
        while (usedSheetNames.Contains(sheetName) || sheetName.Length > sheetNameCharLimit)
        {
            sheetName = baseName[..Math.Min(baseName.Length, 25)] + "_" + suffix++;
        }

        usedSheetNames.Add(sheetName);

        var worksheetPart = spreadsheetDocument.WorkbookPart?.AddNewPart<WorksheetPart>();
        var sheetData = new SheetData();
        if (worksheetPart == null) return;

        worksheetPart.Worksheet = new Worksheet();

        var columns = BuildColumns(result);
        worksheetPart.Worksheet.Append(columns);
        worksheetPart.Worksheet.Append(sheetData);

        var sheets = spreadsheetDocument.WorkbookPart?.Workbook.GetFirstChild<Sheets>() ??
                     spreadsheetDocument.WorkbookPart?.Workbook.AppendChild(new Sheets());

        var relationshipId = spreadsheetDocument.WorkbookPart?.GetIdOfPart(worksheetPart);

        BuildWorksheetHeader(sheetData, filterValue != null ? $"{reportName} - {filterValue}" : $"{reportName}");
        BuildHeaderRow(sheetData, result.Columns);
        BuildDataRows(sheetData, result.Rows);

        if (result.Rows.Count <= 0) return;

        Sheet sheet = new()
        {
            Id = relationshipId,
            SheetId = sheetId,
            Name = sheetName,
        };
        sheets?.Append(sheet);
    }

    private static Columns BuildColumns(DataTable result)
    {
        var columns = new Columns();
        for (var i = 0; i < result.Columns.Count; i++)
        {
            var maxColumnLength = result.AsEnumerable()
                .Max(row => row.Field<object>(result.Columns[i].ColumnName)?.ToString()?.Length);
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

    private static void BuildDataRows(SheetData sheetData, DataRowCollection dataRowCollection)
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
                    CellValue = cellValue?.Length > 0 ? new CellValue(DateTimeExtensions.IsDateTime(cellValue)) : null
                };
                row.AppendChild(cell);
            }

            sheetData.AppendChild(row);
        }
    }

    private static void BuildHeaderRow(SheetData sheetData, DataColumnCollection dataColumnCollection)
    {
        var headerRow = new Row();
        for (var j = 0; j < dataColumnCollection.Count; j++)
        {
            var column = new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(dataColumnCollection[j].ColumnName
                    .SearchAndReplace(new Dictionary<string, string>() { { "_", " " } })),
                StyleIndex = 2
            };
            headerRow.AppendChild(column);
        }

        sheetData.AppendChild(headerRow);
    }

    private static void BuildWorksheetHeader(SheetData sheetData, string reportName)
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

    private static void CreateDataDictionarySheet(SpreadsheetDocument doc, string sheetName, uint sheetId)
    {
        var worksheetPart = doc?.WorkbookPart?.AddNewPart<WorksheetPart>() ??
                            throw new InvalidOperationException(" Workbook part is null");

        var sheetData = new SheetData();

        // Headers row
        var headerRow = new Row();
        headerRow.Append(
            CreateTextCell("A", "Column"),
            CreateTextCell("B", "Heading"),
            CreateTextCell("C", "Description"),
            CreateTextCell("D", "Example Data")
        );
        sheetData.Append(headerRow);

        // Add your rows here:
        var rows = new[]
        {
            ("A", "ODS Code", "A unique code to identify each healthcare organisation in England", "A81021"),
            ("B", "Supplier name", "Attempts to identify GP IT suppliers using a static, rarely updated lookup table.",
                "Egton medical Information Systems Limited (EMIS)"),
            ("C", "Site name", "The name of the practice held within the ODS.", "Normanby Medical Centre"),
            ("D", "Post-code", "The postal code of the practice held within the ODS.", "TS6 6TD"),
            ("E", "ICB", "The parent organisation of the practice.", "NHS Northeast and North Cumbria ICB – 16C (16C)"),
            ("F", "Higher Health Authority", "The parent organisation of the ICB.",
                "NHS Northeast and North Cumbria Integrated Care Board (QHM)"),
            ("G", "Commissioning Region", "The commissioning region of the practice and ICB.",
                "Northeast and Yorkshire Commissioning Region (Y63)"),
            ("H", "Version", "The version of Access Record: Structured deployed in the target GP practice", "v1.5.0"),
            ("I", "Operation", "The FHIR operation called to determine status", "gpc.getstructuredrecord"),
            ("J", "Allergies", "Can the practice flow Allergy info", "Active"),
            ("K", "Medications", "Can the practice flow Medication info", "Inactive"),
            ("L", "Immunisations", "Can the practice flow Immunisation info", "Active"),
            ("M", "Problems", "Can the practice flow Problem info", "Active"),
            ("N", "Consultations", "Can the practice flow Encounter-based info", "Inactive"),
            ("O", "Uncategorised Data", "Flows Observation-based info – e.g., vitals", "Active"),
            ("P", "Investigations", "Flows Investigation-based info – e.g., pathology", "Active"),
            ("Q", "Diary Entries", "Flows Diary entry-based info", "Active"),
            ("R", "Referrals", "Flows Referral entry-based info", "Inactive"),
            ("S", "Documents", "Has Access Record: Document enabled", "Active"),
            ("T", "Documents Version", "Version of Access Record: Documents", "Not available")
        };

        foreach (var (col, heading, desc, example) in rows)
        {
            var row = new Row();
            row.Append(CreateTextCell("A", col));
            row.Append(CreateTextCell("B", heading));
            row.Append(CreateTextCell("C", desc));
            row.Append(CreateTextCell("D", example));
            sheetData.Append(row);
        }

        worksheetPart.Worksheet = new Worksheet(sheetData);

        // Ensure Sheets collection exists before use
        var workbook = doc.WorkbookPart.Workbook;
        if (workbook.Sheets == null)
        {
            workbook.AppendChild(new Sheets());
        }

        var sheets = workbook.Sheets;
        var sheet = new Sheet
        {
            Id = doc.WorkbookPart.GetIdOfPart(worksheetPart),
            SheetId = sheetId,
            Name = sheetName
        };
        sheets.Append(sheet);
    }

    private static Cell CreateTextCell(string column, string text)
    {
        return new Cell
        {
            DataType = CellValues.String,
            CellValue = new CellValue(text)
        };
    }
}