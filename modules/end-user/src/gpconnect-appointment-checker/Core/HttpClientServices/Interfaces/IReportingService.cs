using GpConnect.AppointmentChecker.Models;
using GpConnect.AppointmentChecker.Models.Request;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface IReportingService
{
    Task<DataTable> GetReport(string functionName);
    Task<List<Report>> GetReports();
    Task<List<CapabilityReport>> GetCapabilityReports();
    Task<FileStreamResult> ExportReport(ReportExport reportExport);
}
