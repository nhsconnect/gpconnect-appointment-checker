using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection.Metadata;
using DocumentFormat.OpenXml.Packaging;
using gpconnect_appointment_checker.DTO.Response.Reporting;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IReportingService
    {
        DataTable GetReport(string functionName);
        MemoryStream ExportReport(string functionName, string reportName);
        List<Report> GetReports();
    }
}
