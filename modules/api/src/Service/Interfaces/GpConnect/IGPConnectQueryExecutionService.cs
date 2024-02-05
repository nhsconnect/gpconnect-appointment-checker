using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

public interface IGpConnectQueryExecutionService
{
    Task<SlotSimple> ExecuteFreeSlotSearch(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress, int searchResultId = 0);
    Task<SlotSimple> ExecuteFreeSlotSearchResultFromDatabase(int searchResultId);
    Task<SlotSimple> ExecuteFreeSlotSearchGroupFromDatabase(int searchGroupId);
    //Task<CapabilityStatement> ExecuteFhirCapabilityStatement(RequestParameters requestParameters, string baseAddress);
}
