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

    public GpConnectQueryExecutionService(ISlotSearch slotSearch, ISlotSearchFromDatabase slotSearchFromDatabase, ICapabilityStatement capabilityStatement, ILogService logService)
    {
        _slotSearchFromDatabase = slotSearchFromDatabase;
        _slotSearch = slotSearch;
        _capabilityStatement = capabilityStatement;
        _logService = logService;
    }

    public async Task<SlotSimple> ExecuteFreeSlotSearch(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress, int searchResultId = 0)
    {
        var freeSlots = await _slotSearch.GetFreeSlots(requestParameters, startDate, endDate, baseAddress, searchResultId);
        return freeSlots;
    }

    public async Task<SlotSimple> ExecuteFreeSlotSearchResultFromDatabase(int searchResultId)
    {
        var spineMessage = await _logService.GetSpineMessageLogBySearchResultId(searchResultId);
        if (spineMessage != null)
        {
            var freeSlots = _slotSearchFromDatabase.GetFreeSlotsFromDatabase(spineMessage.ResponsePayload);
            return freeSlots;
        }
        return null;
    }

    public async Task<SlotSimple> ExecuteFreeSlotSearchGroupFromDatabase(int searchGroupId)
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
}
