using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

namespace GpConnect.AppointmentChecker.Api.Service;

public class ExportService : IExportService
{
    private readonly IGpConnectQueryExecutionService _gpConnectQueryExecutionService;
    private readonly IReportingService _reportingService;

    public ExportService(IGpConnectQueryExecutionService gpConnectQueryExecutionService, IReportingService reportingService)
    {
        _reportingService = reportingService;
        _gpConnectQueryExecutionService = gpConnectQueryExecutionService;
    }

    public async Task<Stream> ExportSearchResultFromDatabase(ExportRequest request)
    {
        var result = await _gpConnectQueryExecutionService.ExecuteFreeSlotSearchResultFromDatabase(request.ExportRequestId);
        return GenerateExport(request, result);
    }

    public async Task<Stream> ExportSearchGroupFromDatabase(ExportRequest request)
    {
        var result = await _gpConnectQueryExecutionService.ExecuteFreeSlotSearchGroupFromDatabase(request.ExportRequestId);
        return GenerateExport(request, result);
    }

    private Stream GenerateExport(ExportRequest request, SlotSimple result)
    {
        var json = (result.CurrentSlotEntrySimple.Concat(result.PastSlotEntrySimple)).ConvertObjectToJsonData();
        var dataTable = json.ConvertJsonDataToDataTable();
        var memoryStream = _reportingService.CreateReport(dataTable, request.ReportName);
        return memoryStream;
    }    
}