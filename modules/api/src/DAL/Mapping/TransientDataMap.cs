using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class TransientDataMap : EntityMap<TransientData>
{
    public TransientDataMap()
    {
        Map(p => p.Id).ToColumn("transient_id");
        Map(p => p.Data).ToColumn("transient_data");
        Map(p => p.ReportId).ToColumn("transient_report_id");
        Map(p => p.ReportName).ToColumn("transient_report_name");
    }
}