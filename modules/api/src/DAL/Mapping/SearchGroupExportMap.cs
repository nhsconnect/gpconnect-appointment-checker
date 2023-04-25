using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class SearchGroupExportMap : EntityMap<SearchGroupExport>
{
    public SearchGroupExportMap()
    {
        Map(p => p.SearchResultId).ToColumn("search_result_id");
        Map(p => p.SearchGroupId).ToColumn("search_group_id");
        Map(p => p.ProviderOdsCode).ToColumn("provider_ods_code");
        Map(p => p.ConsumerOdsCode).ToColumn("consumer_ods_code");
        Map(p => p.ProviderOrganisationName).ToColumn("provider_organisation_name");
        Map(p => p.ProviderAddress).ToColumn("provider_address");
        Map(p => p.ProviderPostcode).ToColumn("provider_postcode");
        Map(p => p.ConsumerOrganisationName).ToColumn("consumer_organisation_name");
        Map(p => p.ConsumerAddress).ToColumn("consumer_address");
        Map(p => p.ConsumerPostcode).ToColumn("consumer_postcode");
        Map(p => p.ErrorCode).ToColumn("error_code");
        Map(p => p.Details).ToColumn("details");
        Map(p => p.ProviderPublisher).ToColumn("provider_publisher");
        Map(p => p.ConsumerOrganisationType).ToColumn("consumer_organisation_type");
    }
}
