using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Configuration;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class SpineConfigurationMap : EntityMap<Spine>
    {
        public SpineConfigurationMap()
        {
            Map(p => p.UseSSP).ToColumn("use_ssp");
            Map(p => p.SspHostname).ToColumn("ssp_hostname");
            Map(p => p.SdsHostname).ToColumn("sds_hostname");
            Map(p => p.SdsPort).ToColumn("sds_port");
            Map(p => p.SdsUseLdaps).ToColumn("sds_use_ldaps");
            Map(p => p.OrganisationId).ToColumn("organisation_id");
            Map(p => p.OdsCode).ToColumn("ods_code");
            Map(p => p.OrganisationName).ToColumn("organisation_name");
            Map(p => p.PartyKey).ToColumn("party_key");
            Map(p => p.AsId).ToColumn("asid");
            Map(p => p.TimeoutSeconds).ToColumn("timeout_seconds");
            Map(p => p.ClientCert).ToColumn("client_cert");
            Map(p => p.ClientPrivateKey).ToColumn("client_private_key");
            Map(p => p.ServerCACertChain).ToColumn("server_ca_certchain");
            Map(p => p.SdsUseMutualAuth).ToColumn("sds_use_mutualauth");
            Map(p => p.SpineFqdn).ToColumn("spine_fqdn");
            Map(p => p.SdsTlsVersion).ToColumn("sds_tls_version");
            Map(p => p.SdsUseFhirApi).ToColumn("sds_use_fhir_api");
            Map(p => p.SpineFhirApiDirectoryServicesFqdn).ToColumn("spine_fhir_api_directory_services_fqdn");
            Map(p => p.SpineFhirApiSystemsRegisterFqdn).ToColumn("spine_fhir_api_systems_register_fqdn");
            Map(p => p.SpineFhirApiKey).ToColumn("spine_fhir_api_key");
        }
    }
}
