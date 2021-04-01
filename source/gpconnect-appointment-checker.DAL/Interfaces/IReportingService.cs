using System.Collections.Generic;
using System.Data;
using gpconnect_appointment_checker.DTO.Response.Reporting;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IReportingService
    {
        DataTable GetReport(string functionName);
        void ExportReport(string functionName);
        List<Report> GetReports();
    }
}
