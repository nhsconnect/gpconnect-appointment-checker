using Amazon.SQS.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;
using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using JsonFlatten;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace GpConnect.AppointmentChecker.Api.Service;

public class ReportingService : IReportingService
{
    private readonly ILogger<ReportingService> _logger;
    private readonly ITokenService _tokenService;
    private readonly IDataService _dataService;
    private readonly ISpineService _spineService;
    private readonly IOrganisationService _organisationService;
    private readonly IConfigurationService _configurationService;
    private readonly IMessageService _messageService;
    private readonly ICapabilityStatement _capabilityStatement;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReportingService(ILogger<ReportingService> logger, IConfigurationService configurationService, IMessageService messageService, IDataService dataService, ISpineService spineService, IOrganisationService organisationService, ICapabilityStatement capabilityStatement, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenService = tokenService;
        _spineService = spineService;
        _organisationService = organisationService;
        _configurationService = configurationService;
        _capabilityStatement = capabilityStatement;
        _messageService = messageService;
        _dataService = dataService;
        _httpContextAccessor = httpContextAccessor;
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
        DataTable? dataTable = await _dataService.ExecuteFunctionAndGetDataTable($"reporting.{reportRequest.FunctionName}", null);
        var memoryStream = CreateReport(dataTable, reportRequest.ReportName);
        return memoryStream;
    }

    public async Task<Stream> CreateInteractionReport(ReportCreationRequest reportCreationRequest)
    {
        var memoryStream = CreateReport(reportCreationRequest.JsonData.ConvertJsonDataToDataTable(), reportCreationRequest.ReportName);
        return memoryStream;
    }

    public async Task<string> CreateInteractionData(ReportInteractionRequest reportInteractionRequest)
    {
        try
        {
            var odsCodesInScope = reportInteractionRequest.ReportSource.DistinctBy(x => x.OdsCode).Select(x => x.OdsCode).ToList();
            string? jsonData = null;
            var organisationHierarchy = await _organisationService.GetOrganisationHierarchy(odsCodesInScope);
            var capabilityStatements = new List<IDictionary<string, object>>();

            if (odsCodesInScope.Count > 0)
            {
                for (var i = 0; i < odsCodesInScope.Count; i++)
                {
                    _logger.LogInformation("Dumping out reportInteractionRequest in CreateInteractionData");
                    _logger.LogInformation(reportInteractionRequest.InteractionId);
                    _logger.LogInformation(reportInteractionRequest.ReportName);
                    _logger.LogInformation(reportInteractionRequest.MessageGroupId.ToString());
                    _logger.LogInformation(reportInteractionRequest.ReportSource[0].OdsCode);
                    _logger.LogInformation(reportInteractionRequest.ReportSource[0].SupplierName);

                    var capabilityStatementReporting = new CapabilityStatementReporting()
                    {
                        Hierarchy = organisationHierarchy[odsCodesInScope[i]],
                        SupplierName = reportInteractionRequest.ReportSource[i].SupplierName
                    };

                    _logger.LogInformation("Getting Provider Details");
                    _logger.LogInformation(odsCodesInScope[i]);
                    _logger.LogInformation(reportInteractionRequest.InteractionId);

                    var providerSpineDetails = await _spineService.GetProviderDetails(odsCodesInScope[i], reportInteractionRequest.InteractionId);

                    if (providerSpineDetails != null)
                    {
                        _logger.LogInformation("providerSpineDetails != null");

                        var spineMessageType = await _configurationService.GetSpineMessageType(SpineMessageTypes.GpConnectReadMetaData, reportInteractionRequest.InteractionId);

                        var requestParameters = await _tokenService.ConstructRequestParameters(new DTO.Request.GpConnect.RequestParameters()
                        {
                            RequestUri = new Uri($"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}"),
                            ProviderSpineDetails = new SpineProviderRequestParameters() { EndpointAddress = providerSpineDetails.EndpointAddress, AsId = providerSpineDetails.AsId },
                            ProviderOrganisationDetails = new OrganisationRequestParameters() { OdsCode = odsCodesInScope[i] },
                            SpineMessageTypeId = (SpineMessageTypes)spineMessageType.SpineMessageTypeId,
                            Sid = Guid.NewGuid().ToString()
                        });

                        if (requestParameters != null)
                        {
                            var capabilityStatement = await _capabilityStatement.GetCapabilityStatement(requestParameters, providerSpineDetails.SspHostname, reportInteractionRequest.InteractionId);

                            if (capabilityStatement != null && capabilityStatement.NoIssues)
                            {
                                capabilityStatementReporting.Profile = capabilityStatement.Profile;
                                capabilityStatementReporting.Version = $"v{capabilityStatement.Version}";
                                capabilityStatementReporting.Rest = capabilityStatement.Rest.FirstOrDefault()?.Operation.Select(x => x.Name);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogInformation("providerSpineDetails are null");
                    }

                    var jsonString = JsonConvert.SerializeObject(capabilityStatementReporting);
                    var jObject = JObject.Parse(jsonString);
                    capabilityStatements.Add(jObject.Flatten());
                }
                jsonData = JsonConvert.SerializeObject(capabilityStatements);
            }
            return jsonData.Substring(1, jsonData.Length - 2);
        }
        catch (Exception exc)
        {
            _logger?.LogError(exc, "An error has occurred while attempting to execute the function 'CreateInteractionData'");
            throw;
        }
    }

    public async Task<MemoryStream> ExportBySpineMessage(int spineMessageId, string reportName)
    {
        var parameters = new Dictionary<string, int>
            {
                { "_user_id", LoggingHelper.GetIntegerValue(Helpers.Constants.Headers.UserId) },
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

    public async Task<List<CapabilityReport>> GetCapabilityReports()
    {
        var functionName = "reporting.get_capability_reports";
        var result = await _dataService.ExecuteQuery<CapabilityReport>(functionName);
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
            Name = reportName.SearchAndReplace(new Dictionary<string, string>() { { ":", string.Empty } })
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
                    CellValue = cellValue?.Length > 0 ? new CellValue(IsDateTime(cellValue)) : null
                };
                row.AppendChild(cell);
            }
            sheetData.AppendChild(row);
        }
    }

    private string IsDateTime(string cellValue)
    {
        DateTime dateValue;
        return DateTime.TryParse(cellValue, out dateValue) ? dateValue.ToString("dd/MMM/yyyy HH:mm:ss") : cellValue;
    }

    private void BuildHeaderRow(SheetData sheetData, DataColumnCollection dataColumnCollection)
    {
        var headerRow = new Row();
        for (var j = 0; j < dataColumnCollection.Count; j++)
        {
            var column = new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(dataColumnCollection[j].ColumnName.SearchAndReplace(new Dictionary<string, string>() { { "_", " " } })),
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
