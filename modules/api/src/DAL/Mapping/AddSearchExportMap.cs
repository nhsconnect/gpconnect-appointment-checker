using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class AddSearchExportMap : EntityMap<AddSearchExport>
{
    public AddSearchExportMap()
    {
        Map(p => p.SearchExportId).ToColumn("search_export_id");
    }
}
