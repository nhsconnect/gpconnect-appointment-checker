using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Configuration;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class SpineMap : EntityMap<Spine>
    {
        public SpineMap()
        {
            Map(p => p.UseSSP).ToColumn("use_ssp");
            Map(p => p.SSPHostname).ToColumn("ssp_hostname");
            Map(p => p.SDSHostname).ToColumn("sds_hostname");
            Map(p => p.SDSPort).ToColumn("sds_port");
            Map(p => p.SDSUseLdaps).ToColumn("sds_use_ldaps");
            Map(p => p.OrganisationId).ToColumn("organisation_id");
            Map(p => p.PartyKey).ToColumn("party_key");
            Map(p => p.AsId).ToColumn("asid");
        }
    }
}
