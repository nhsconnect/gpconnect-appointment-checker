using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Configuration;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class FhirApiQueryMap : EntityMap<FhirApiQuery>
    {
        public FhirApiQueryMap()
        {
            Map(p => p.QueryName).ToColumn("query_name");
            Map(p => p.QueryText).ToColumn("query_text");
        }
    }
}
