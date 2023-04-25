using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class OrganisationTypeMap : EntityMap<OrganisationType>
{
    public OrganisationTypeMap()
    {
        Map(p => p.OrganisationTypeId).ToColumn("organisation_type_id");
        Map(p => p.OrganisationTypeCode).ToColumn("organisation_type_code");
        Map(p => p.OrganisationTypeDescription).ToColumn("organisation_type_description");
    }
}
