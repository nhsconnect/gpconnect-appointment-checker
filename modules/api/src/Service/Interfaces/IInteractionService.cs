using GpConnect.AppointmentChecker.Api.DTO.Request;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IInteractionService
{
    public Task<string> CreateInteractionData<T>(RouteReportRequest routeReportRequest) where T : class;
}
