using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class CapabilityReportMap : EntityMap<CapabilityReport>
{
    public CapabilityReportMap()
    {
        Map(p => p.ReportName).ToColumn("report_name");
    }
}
