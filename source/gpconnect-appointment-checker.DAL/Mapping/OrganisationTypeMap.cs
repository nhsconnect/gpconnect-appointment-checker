using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Configuration;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class OrganisationTypeMap : EntityMap<OrganisationType>
    {
        public OrganisationTypeMap()
        {
            Map(p => p.OrganisationTypeId).ToColumn("organisation_type_id");
            Map(p => p.OrganisationTypeCode).ToColumn("organisation_type_code");
            Map(p => p.OrganisationTypeDescription).ToColumn("organisation_type_description");
        }
    }
}
