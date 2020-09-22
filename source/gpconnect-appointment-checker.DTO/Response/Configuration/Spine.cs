using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Response.Configuration
{
    public class Spine
    {
        public bool UseSSP { get; set; }
        [JsonProperty("nhsMHSEndPoint")]
        public string SSPHostname { get; set; }
        public string SDSHostname { get; set; }
        public int SDSPort { get; set; }
        public bool SDSUseLdaps { get; set; }
        public int OrganisationId { get; set; }
        [JsonProperty("nhsMHSPartyKey")]
        public string PartyKey { get; set; }
        [JsonProperty("uniqueIdentifier")]
        public string AsId { get; set; }
    }
}
