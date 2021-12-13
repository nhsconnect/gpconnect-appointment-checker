using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Response.Fhir
{
    public class Coding
    {
        [JsonProperty("system")]
        public string System { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("display")]
        public string Display { get; set; }
    }
}
