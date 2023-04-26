using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class SearchExportMap : EntityMap<SearchExport>
{
    public SearchExportMap()
    {
        Map(p => p.SearchExportId).ToColumn("search_export_id");
        Map(p => p.SearchExportData).ToColumn("search_export_data");
    }
}
