using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Ldap
{
    public class Spine
    {
        [JsonProperty("uniqueIdentifier")]
        public string AsId { get; set; }
        [JsonProperty("nhsMHSPartyKey")]
        public string PartyKey { get; set; }
        [JsonProperty("nhsMHSEndPoint")] 
        public string EndpointAddress { get; set; }
        [JsonProperty("nhsProductName")]
        public string ProductName { get; set; }
    }
}
