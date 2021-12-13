using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Response.Fhir
{
    public class Issue
    {
        [JsonProperty("severity")]
        public string Severity { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("details")]
        public Detail Details { get; set; }
        [JsonProperty("diagnostics")]
        public string Diagnostics { get; set; }
    }
}
