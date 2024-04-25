using GpConnect.AppointmentChecker.Api.DTO.Request;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IWorkflowService
{
    public Task<string> CreateWorkflowData(RouteReportRequest routeReportRequest);
}