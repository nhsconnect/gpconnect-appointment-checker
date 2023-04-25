using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

public interface ISlotSearchFromDatabase
{
    SlotSimple GetFreeSlotsFromDatabase(string responseStream);
}
