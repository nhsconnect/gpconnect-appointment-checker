using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class OrganisationMap : EntityMap<Organisation>
{
    public OrganisationMap()
    {
        Map(p => p.OrganisationId).ToColumn("organisation_id");
        Map(p => p.ODSCode).ToColumn("ods_code");
        Map(p => p.OrganisationName).ToColumn("organisation_name");
        Map(p => p.AddressLine1).ToColumn("address_line_1");
        Map(p => p.AddressLine2).ToColumn("address_line_2");
        Map(p => p.Locality).ToColumn("locality");
        Map(p => p.City).ToColumn("city");
        Map(p => p.County).ToColumn("county");
        Map(p => p.Postcode).ToColumn("postcode");
    }
}
