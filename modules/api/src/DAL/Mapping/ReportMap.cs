using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class ReportMap : EntityMap<Report>
{
    public ReportMap()
    {
        Map(p => p.ReportName).ToColumn("report_name");
        Map(p => p.FunctionName).ToColumn("function_name");
    }
}
