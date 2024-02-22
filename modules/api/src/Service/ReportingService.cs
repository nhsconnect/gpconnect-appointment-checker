using Amazon.SQS.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;
using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;
using GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.GpConnect;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using JsonFlatten;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Reflection.Emit;
using System.Threading;

namespace GpConnect.AppointmentChecker.Api.Service;

public class ReportingService : IReportingService
{
    private readonly ILogger<ReportingService> _logger;
    private readonly ITokenService _tokenService;
    private readonly IDataService _dataService;
    private readonly ISpineService _spineService;
    private readonly IOrganisationService _organisationService;
    private readonly IMessageService _messageService;
    private readonly ICapabilityStatement _capabilityStatement;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReportingService(ILogger<ReportingService> logger, IMessageService messageService, IDataService dataService, ISpineService spineService, IOrganisationService organisationService, ICapabilityStatement capabilityStatement, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenService = tokenService;
        _spineService = spineService;
        _organisationService = organisationService;
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

        await _messageService.SendMessageToQueue(new SendMessageRequest() { 
            MessageGroupId = reportInteractionRequest.MessageGroupId.ToString(), 
            MessageBody = request }
        );
    }

    public async Task CreateCompletionMessage(ReportCompletionRequest reportCompletionRequest)
    {
        var request = JsonConvert.SerializeObject(reportCompletionRequest, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        });

        await _messageService.SendMessageToQueue(new SendMessageRequest()
        {
            MessageBody = request
        }
        );
    }

    public async Task<Stream> ExportReport(ReportRequest reportRequest)
    {
        DataTable? dataTable = await _dataService.ExecuteFunctionAndGetDataTable($"reporting.{reportRequest.FunctionName}", null);
        var memoryStream = CreateReport(dataTable, reportRequest.ReportName);
        return memoryStream;
    }

    public async Task<Stream> CreateInteractionReport(ReportInteractionRequest reportInteractionRequest)
    {
        reportInteractionRequest.OdsCodes = reportInteractionRequest.OdsCodes.DistinctBy(x => x).Take(10).ToList();
        string? jsonData = null;
        var organisationHierarchy = await _organisationService.GetOrganisationHierarchy(reportInteractionRequest.OdsCodes);

        if (reportInteractionRequest.OdsCodes.Count > 0)
        {
            int numberOfRequests = reportInteractionRequest.OdsCodes.Count;
            int maxParallelRequests = 100;
            var semaphoreSlim = new SemaphoreSlim(maxParallelRequests, numberOfRequests);

            var capabilityReportInstance = new List<Task<IDictionary<string, object>>>();

            for (int i = 0; i < numberOfRequests; ++i)
            {
                //var hierarchy = organisationHierarchy.FirstOrDefault(x => x.OdsCode == reportInteractionRequest.OdsCodes[i]);
                //capabilityReportInstance.Add(GetCapabilityReportInstance(reportInteractionRequest.OdsCodes[i], reportInteractionRequest.InteractionId, hierarchy, semaphoreSlim));
            }
            var capabilityStatements = await Task.WhenAll(capabilityReportInstance.ToArray());
            jsonData = JsonConvert.SerializeObject(capabilityStatements);
        }
        
        var memoryStream = CreateReport(jsonData.ConvertJsonDataToDataTable(), reportInteractionRequest.ReportName);
        return memoryStream;
    }

    public async Task<string> CreateInteractionData(ReportInteractionRequest reportInteractionRequest)
    {
        reportInteractionRequest.OdsCodes = reportInteractionRequest.OdsCodes.DistinctBy(x => x).ToList();
        string? jsonData = null;
        var organisationHierarchy = await _organisationService.GetOrganisationHierarchy(reportInteractionRequest.OdsCodes);
        var capabilityStatements = new List<IDictionary<string, object>>();

        if (reportInteractionRequest.OdsCodes.Count > 0)
        {
            for(var i = 0; i < reportInteractionRequest.OdsCodes.Count; i++)
            {
                var capabilityStatementReporting = new CapabilityStatementReporting()
                {
                    Hierarchy = organisationHierarchy[reportInteractionRequest.OdsCodes[i]]
                };

                _logger.LogInformation($"Checking spine details for {reportInteractionRequest.OdsCodes[i]}");

                var providerSpineDetails = await _spineService.GetProviderDetails(reportInteractionRequest.OdsCodes[i]);               

                if (providerSpineDetails != null)
                {
                    _logger.LogInformation($"providerSpineDetails.OdsCode are {providerSpineDetails.OdsCode}");
                    _logger.LogInformation($"providerSpineDetails.EndpointAddress are {providerSpineDetails.EndpointAddress}");

                    var requestParameters = await _tokenService.ConstructRequestParameters(new DTO.Request.GpConnect.RequestParameters()
                    {
                        RequestUri = new Uri($"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}"),
                        ProviderSpineDetails = new SpineProviderRequestParameters() { EndpointAddress = providerSpineDetails.EndpointAddress, AsId = providerSpineDetails.AsId },
                        ProviderOrganisationDetails = new OrganisationRequestParameters() { OdsCode = reportInteractionRequest.OdsCodes[i] },
                        SpineMessageTypeId = SpineMessageTypes.GpConnectReadMetaData,
                        Sid = Guid.NewGuid().ToString()
                    });

                    if (requestParameters != null)
                    {
                        _logger.LogInformation($"requestParameters.SpineMessageTypeId is {requestParameters.SpineMessageTypeId}");
                        _logger.LogInformation($"requestParameters.SspFrom is {requestParameters.SspFrom}");
                        _logger.LogInformation($"requestParameters.SspTo is {requestParameters.SspTo}");
                        _logger.LogInformation($"requestParameters.InteractionId is {requestParameters.InteractionId}");
                        _logger.LogInformation($"requestParameters.BearerToken is {requestParameters.BearerToken}");
                        _logger.LogInformation($"requestParameters.EndpointAddressWithSpineSecureProxy is {requestParameters.EndpointAddressWithSpineSecureProxy}");

                        var capabilityStatement = await _capabilityStatement.GetCapabilityStatement(requestParameters, providerSpineDetails.SspHostname);

                        _logger.LogInformation($"capabilityStatement is {providerSpineDetails.OdsCode}");
                        _logger.LogInformation($"providerSpineDetails.OrganisationName are {providerSpineDetails.OrganisationName}");

                        if (capabilityStatement != null && capabilityStatement.NoIssues)
                        {
                            _logger.LogInformation($"capabilityStatementReporting.Profile are {capabilityStatement.Profile}");
                            _logger.LogInformation($"capabilityStatementReporting.Version are {capabilityStatement.Version}");

                            capabilityStatementReporting.Profile = capabilityStatement.Profile;
                            capabilityStatementReporting.Version = $"v{capabilityStatement.Version}";
                            capabilityStatementReporting.Rest = capabilityStatement.Rest.FirstOrDefault()?.Operation.Select(x => x.Name);
                        }
                        else
                        {
                            _logger.LogInformation($"No capabilityStatement for {reportInteractionRequest.OdsCodes[i]}");
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"No requestParameters for {reportInteractionRequest.OdsCodes[i]}");
                    }
                }
                else
                {
                    _logger.LogInformation($"No providerSpineDetails for {reportInteractionRequest.OdsCodes[i]}");
                }

                var jsonString = JsonConvert.SerializeObject(capabilityStatementReporting);
                var jObject = JObject.Parse(jsonString);
                capabilityStatements.Add(jObject.Flatten());
            }

            jsonData = JsonConvert.SerializeObject(capabilityStatements);
        }
        return jsonData;
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
