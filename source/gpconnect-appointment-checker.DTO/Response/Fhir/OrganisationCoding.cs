using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Response.Fhir
{
    public class OrganisationCoding
    {
        [JsonProperty("display")]
        public string OrganisationTypeDisplay { get; set; }
    }
}
