using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Application;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class OrganisationMap : EntityMap<Organisation>
    {
        public OrganisationMap()
        {
            Map(p => p.OrganisationId).ToColumn("organisation_id");
            Map(p => p.OdsCode).ToColumn("ods_code");
            Map(p => p.OrganisationTypeCode).ToColumn("organisation_type_name");
            Map(p => p.OrganisationName).ToColumn("organisation_name");
            Map(p => p.PostalAddress).ToColumn("address_line_1");
            Map(p => p.PostalCode).ToColumn("postcode");
        }
    }
}
