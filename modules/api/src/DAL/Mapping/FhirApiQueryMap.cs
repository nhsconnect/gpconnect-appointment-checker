using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class FhirApiQueryMap : EntityMap<FhirApiQuery>
{
    public FhirApiQueryMap()
    {
        Map(p => p.QueryText).ToColumn("query_text");
        Map(p => p.QueryName).ToColumn("query_name");
    }
}
