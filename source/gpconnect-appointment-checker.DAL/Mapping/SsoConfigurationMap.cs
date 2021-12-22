using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Configuration;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class SsoConfigurationMap : EntityMap<Sso>
    {
        public SsoConfigurationMap()
        {
            Map(p => p.ClientId).ToColumn("client_id");
            Map(p => p.ClientSecret).ToColumn("client_secret");
            Map(p => p.CallbackPath).ToColumn("callback_path");
            Map(p => p.AuthScheme).ToColumn("auth_scheme");
            Map(p => p.ChallengeScheme).ToColumn("challenge_scheme");
            Map(p => p.AuthEndpoint).ToColumn("auth_endpoint");
            Map(p => p.TokenEndpoint).ToColumn("token_endpoint");
            Map(p => p.MetadataEndpoint).ToColumn("metadata_endpoint");
            Map(p => p.SignedOutCallbackPath).ToColumn("signed_out_callback_path");
        }
    }
}
