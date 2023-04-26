using GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IReportingService
{
    public Task<string> GetReport(string functionName);
    public Task<MemoryStream> ExportByReportName(string functionName, string reportName);
    public Task<List<Report>> GetReports();
    public Task<MemoryStream> ExportBySpineMessage(int spineMessageId, int userId, string reportName);
}
