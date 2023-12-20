using Dapper.FluentMap;
using GpConnect.AppointmentChecker.Api.DAL.Mapping;

namespace GpConnect.AppointmentChecker.Api.Core.Mapping;

public static class MappingExtensions
{
    public static void ConfigureMappingServices()
    {
        FluentMapper.Initialize(config =>
        {
            config.AddMap(new UserMap());
            config.AddMap(new EmailTemplateMap());
            config.AddMap(new OrganisationMap());
            config.AddMap(new FhirApiQueryMap());
            config.AddMap(new SdsQueryMap());
            //config.AddMap(new SpineMap());
            config.AddMap(new OrganisationTypeMap());
            config.AddMap(new SpineMessageTypeMap());
            config.AddMap(new SearchExportMap());
            config.AddMap(new SearchGroupExportMap());
            config.AddMap(new SearchGroupMap());            
            config.AddMap(new SearchResultMap());
            config.AddMap(new AddSearchResultMap());
            config.AddMap(new AddSearchExportMap());
            config.AddMap(new SearchResultByGroupMap());
            config.AddMap(new ReportMap());
            config.AddMap(new SpineMessageMap());
        });
    }
}
