using GpConnect.AppointmentChecker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface IReportingService
{
    Task<string> GetReport(string functionName);
    Task<List<Report>> GetReports();
    Task<string> ExportReport(string functionName, string reportName);
}
