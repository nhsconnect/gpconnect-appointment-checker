using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Configuration;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class SpineMap : EntityMap<Spine>
    {
        public SpineMap()
        {
            Map(p => p.use_ssp).ToColumn("use_ssp");
            Map(p => p.ssp_hostname).ToColumn("ssp_hostname");
            Map(p => p.sds_hostname).ToColumn("sds_hostname");
            Map(p => p.sds_port).ToColumn("sds_port");
            Map(p => p.sds_use_ldaps).ToColumn("sds_use_ldaps");
            Map(p => p.organisation_id).ToColumn("organisation_id");
            Map(p => p.party_key).ToColumn("party_key");
            Map(p => p.asid).ToColumn("asid");
            Map(p => p.timeout_seconds).ToColumn("timeout_seconds");
        }
    }
}
