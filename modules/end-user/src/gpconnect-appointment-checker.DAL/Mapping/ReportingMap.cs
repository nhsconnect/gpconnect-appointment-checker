using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Reporting;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class ReportingMap : EntityMap<Report>
    {
        public ReportingMap()
        {
            Map(p => p.ReportName).ToColumn("report_name");
            Map(p => p.FunctionName).ToColumn("function_name");
        }
    }
}
