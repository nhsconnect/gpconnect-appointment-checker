using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;
using System.Data;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IReportingService
{
    public Task<string> GetReport(string functionName);
    public Task<Stream> ExportReport(ReportRequest reportRequest);
    public Task<Stream> CreateInteractionReport(ReportCreationRequest reportCreationRequest);
    public Task RouteReportRequest(RouteReportRequest routeReportRequest);
    public Task SendMessageToCreateInteractionReportContent(ReportInteractionRequest reportInteractionRequest);
    public Task<List<Report>> GetReports();    
    public Task<List<CapabilityReport>> GetCapabilityReports();
    public Task<MemoryStream> ExportBySpineMessage(int spineMessageId, string reportName);
    public MemoryStream CreateReport(DataTable result, string reportName, List<ReportFilterRequest>? reportFilterRequest = null);
}
