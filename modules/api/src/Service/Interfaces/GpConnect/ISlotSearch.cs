using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

public interface ISlotSearch
{
    Task<SlotSimple> GetFreeSlots(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress, int searchResultId = 0);
}
