using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

namespace GpConnect.AppointmentChecker.Api.Service.GpConnect;

public class GpConnectQueryExecutionService : IGpConnectQueryExecutionService
{
    private readonly ISlotSearch _slotSearch;
    private readonly ISlotSearchFromDatabase _slotSearchFromDatabase;
    private readonly ICapabilityStatement _capabilityStatement;
    private readonly ILogService _logService;
    private readonly IApplicationService _applicationService;

    public GpConnectQueryExecutionService(ISlotSearch slotSearch, ISlotSearchFromDatabase slotSearchFromDatabase, ICapabilityStatement capabilityStatement, ILogService logService, IApplicationService applicationService)
    {
        _slotSearchFromDatabase = slotSearchFromDatabase;
        _slotSearch = slotSearch;
        _capabilityStatement = capabilityStatement;
        _logService = logService;
        _applicationService = applicationService;
    }

    public async Task<SlotSimple> ExecuteFreeSlotSearch(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress, int userId, int searchResultId = 0)
    {
        var freeSlots = await _slotSearch.GetFreeSlots(requestParameters, startDate, endDate, baseAddress, searchResultId);
        return freeSlots;
    }

    public async Task<SlotSimple> ExecuteFreeSlotSearchResultFromDatabase(int searchResultId, int userId)
    {
        var spineMessage = await _logService.GetSpineMessageLogBySearchResultId(searchResultId);
        if (spineMessage != null)
        {
            var freeSlots = _slotSearchFromDatabase.GetFreeSlotsFromDatabase(spineMessage.ResponsePayload);
            return freeSlots;
        }
        return null;
    }

    public async Task<SlotSimple> ExecuteFreeSlotSearchGroupFromDatabase(int searchGroupId, int userId)
    {
        var slotSimple = new SlotSimple()
        {
            CurrentSlotEntrySimple = new List<SlotEntrySimple>(),
            PastSlotEntrySimple = new List<SlotEntrySimple>()
        };
        var spineMessage = await _logService.GetSpineMessageLogBySearchGroupId(searchGroupId);
        for(var i = 0; i < spineMessage.Count;i++)
        {
            var freeSlots = _slotSearchFromDatabase.GetFreeSlotsFromDatabase(spineMessage[i].ResponsePayload);
            slotSimple.CurrentSlotEntrySimple.AddRange(freeSlots.CurrentSlotEntrySimple.OrderBy(x => x.LocationName));
            slotSimple.PastSlotEntrySimple.AddRange(freeSlots.PastSlotEntrySimple.OrderBy(x => x.LocationName));
        }
        return slotSimple;
    }

    public async Task<DTO.Response.GpConnect.CapabilityStatement> ExecuteFhirCapabilityStatement(RequestParameters requestParameters, string baseAddress)
    {
        var capabilityStatement = await _capabilityStatement.GetCapabilityStatement(requestParameters, baseAddress);
        return capabilityStatement;
    }
}
