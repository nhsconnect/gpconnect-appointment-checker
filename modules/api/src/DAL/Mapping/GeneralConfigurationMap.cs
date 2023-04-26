using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class GeneralConfigurationMap : EntityMap<General>
{
    public GeneralConfigurationMap()
    {
        Map(p => p.ProductName).ToColumn("product_name");
        Map(p => p.ProductVersion).ToColumn("product_version");
        Map(p => p.MaxNumWeeksSearch).ToColumn("max_num_weeks_search");
        Map(p => p.LogRetentionDays).ToColumn("log_retention_days");
        Map(p => p.GetAccessEmailAddress).ToColumn("get_access_email_address");
        Map(p => p.MaxNumberProviderCodesSearch).ToColumn("max_number_provider_codes_search");
        Map(p => p.MaxNumberConsumerCodesSearch).ToColumn("max_number_consumer_codes_search");
    }
}

