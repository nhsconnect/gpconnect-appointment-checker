using GpConnect.AppointmentChecker.Api.DTO.Request;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IWorkflowService
{
    public Task<string> CreateWorkflowData(RouteReportRequest routeReportRequest);
    public Task<DTO.Response.Mesh.Root?> GetWorkflowData(string workflow, string odsCode);
}