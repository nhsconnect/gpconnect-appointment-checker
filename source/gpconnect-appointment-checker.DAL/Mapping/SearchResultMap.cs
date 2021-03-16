using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Application;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class SearchResultMap : EntityMap<SearchResult>
    {
        public SearchResultMap()
        {
            Map(p => p.SearchResultId).ToColumn("search_result_id");
            Map(p => p.SearchGroupId).ToColumn("search_group_id");
            Map(p => p.ResponsePayload).ToColumn("response_payload");
        }
    }
}
