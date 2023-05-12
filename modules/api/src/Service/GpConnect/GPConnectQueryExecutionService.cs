using GpConnect.AppointmentChecker.Api.DTO.Request.Application;
using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

namespace GpConnect.AppointmentChecker.Api.Service.GpConnect;

public class GpConnectQueryExecutionService : IGpConnectQueryExecutionService
{
    private readonly ILogger<GpConnectQueryExecutionService> _logger;
    private readonly IAuditService _auditService;
    private readonly IApplicationService _applicationService;
    private readonly ISlotSearch _slotSearch;
    private readonly ISlotSearchFromDatabase _slotSearchFromDatabase;
    private readonly ICapabilityStatement _capabilityStatement;
    private readonly ILogService _logService;

    public GpConnectQueryExecutionService(ILogger<GpConnectQueryExecutionService> logger, IApplicationService applicationService, IAuditService auditService, ISlotSearch slotSearch, ISlotSearchFromDatabase slotSearchFromDatabase, ICapabilityStatement capabilityStatement, ILogService logService)
    {
        _logger = logger;
        _applicationService = applicationService;
        _auditService = auditService;
        _slotSearchFromDatabase = slotSearchFromDatabase;
        _slotSearch = slotSearch;
        _capabilityStatement = capabilityStatement;
        _logService = logService;
    }

    public async Task<SlotSimple> ExecuteFreeSlotSearch(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress, int userId, int searchResultId = 0)
    {
        var freeSlots = await _slotSearch.GetFreeSlots(requestParameters, startDate, endDate, baseAddress, searchResultId);

        var searchExport = new SearchExport
        {
            SearchExportData = freeSlots.ExportStreamData,
            UserId = userId
        };

        var searchExportInstance = await _applicationService.AddSearchExport(searchExport);
        freeSlots.SearchExportId = searchExportInstance.SearchExportId;
        return freeSlots;
    }

    public async Task<SlotSimple> ExecuteFreeSlotSearchFromDatabase(int searchResultId, int userId)
    {
        var spineMessage = await _logService.GetSpineMessageLogBySearchResultId(searchResultId);
        if (spineMessage != null)
        {
            var freeSlots = _slotSearchFromDatabase.GetFreeSlotsFromDatabase(spineMessage.ResponsePayload);

            var searchExport = new SearchExport
            {
                SearchExportData = freeSlots.ExportStreamData,
                UserId = userId
            };

            var searchExportInstance = await _applicationService.AddSearchExport(searchExport);
            freeSlots.SearchExportId = searchExportInstance.SearchExportId;
            return freeSlots;
        }
        return null;
    }

    public async Task<DTO.Response.GpConnect.CapabilityStatement> ExecuteFhirCapabilityStatement(RequestParameters requestParameters, string baseAddress)
    {
        var capabilityStatement = await _capabilityStatement.GetCapabilityStatement(requestParameters, baseAddress);
        return capabilityStatement;
    }
}
