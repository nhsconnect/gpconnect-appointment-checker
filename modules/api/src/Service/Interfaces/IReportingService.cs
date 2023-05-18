using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;
using System.Data;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IReportingService
{
    public Task<string> GetReport(string functionName);
    public Task<Stream> ExportReport(ReportRequest reportRequest);
    public Task<List<Report>> GetReports();
    public Task<MemoryStream> ExportBySpineMessage(int spineMessageId, int userId, string reportName);
    public MemoryStream CreateReport(DataTable result, string reportName);
}
