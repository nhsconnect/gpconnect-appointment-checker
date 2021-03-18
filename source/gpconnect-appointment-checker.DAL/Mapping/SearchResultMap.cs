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
            Map(p => p.ProviderOdsCode).ToColumn("provider_ods_code");
            Map(p => p.ConsumerOdsCode).ToColumn("consumer_ods_code");
            Map(p => p.ProviderOrganisationName).ToColumn("provider_organisation_name");
            Map(p => p.ProviderAddress).ToColumn("provider_address");
            Map(p => p.ProviderPostcode).ToColumn("provider_postcode");
            Map(p => p.ConsumerOrganisationName).ToColumn("consumer_organisation_name");
            Map(p => p.ConsumerAddress).ToColumn("consumer_address");
            Map(p => p.ConsumerPostcode).ToColumn("consumer_postcode");
            Map(p => p.ProviderPublisher).ToColumn("provider_publisher");
            Map(p => p.SearchDurationSeconds).ToColumn("search_duration_seconds");
        }
    }
}
