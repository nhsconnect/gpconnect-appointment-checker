using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Application;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class SearchExportMap : EntityMap<SearchExport>
    {
        public SearchExportMap()
        {
            Map(p => p.SearchExportId).ToColumn("search_export_id");
            Map(p => p.SearchExportData).ToColumn("search_export_data");
        }
    }
}
