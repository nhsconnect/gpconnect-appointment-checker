using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Configuration;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class SdsQueryMap : EntityMap<SdsQuery>
    {
        public SdsQueryMap()
        {
            Map(p => p.QueryName).ToColumn("query_name");
            Map(p => p.SearchBase).ToColumn("search_base");
            Map(p => p.QueryText).ToColumn("query_text");
            Map(p => p.QueryAttributes).ToColumn("query_attributes");
        }
    }
}
