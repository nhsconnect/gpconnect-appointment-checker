using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Response.Configuration
{
    public class Spine
    {
        public bool use_ssp { get; set; }
        [JsonProperty("nhsMHSEndPoint")]
        public string ssp_hostname { get; set; }
        public string sds_hostname { get; set; }
        public string client_cert { get; set; }
        public string client_private_key { get; set; }
        public string server_ca_certchain { get; set; }
        public int sds_port { get; set; }
        public bool sds_use_ldaps { get; set; }
        public int organisation_id { get; set; }
        [JsonProperty("nhsMHSPartyKey")]
        public string party_key { get; set; }
        [JsonProperty("uniqueIdentifier")]
        public string asid { get; set; }
        public int timeout_seconds { get; set; }
    }
}
