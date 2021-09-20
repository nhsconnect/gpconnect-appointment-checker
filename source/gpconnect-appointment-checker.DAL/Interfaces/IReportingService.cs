using gpconnect_appointment_checker.DTO.Response.Reporting;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IReportingService
    {
        DataTable GetReport(string functionName);
        MemoryStream ExportReport(string functionName, string reportName);
        MemoryStream ExportReport(int spineMessageId);
        MemoryStream CreateReport(DataTable result, string reportName = "");
        List<Report> GetReports();
    }
}
