using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class SdsQueryMap : EntityMap<SdsQuery>
{
    public SdsQueryMap()
    {
        Map(p => p.QueryText).ToColumn("query_text");
        Map(p => p.QueryName).ToColumn("query_name");
        Map(p => p.SearchBase).ToColumn("search_base");
        Map(p => p.QueryAttributes).ToColumn("query_attributes");
    }
}
