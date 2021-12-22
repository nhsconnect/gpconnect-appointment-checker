using Newtonsoft.Json;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Response.Fhir
{
    public class Organisation
    {
        [JsonProperty("id")]
        public string OrganisationId { get; set; }
        [JsonProperty("name")]
        public string OrganisationName { get; set; }
        [JsonProperty("address")]
        public PostalAddress PostalAddress { get; set; }
        [JsonProperty("type")]
        public Type Type { get; set; }
        [JsonProperty("issue")]
        public List<Issue> Issue { get; set; }

        [JsonProperty("errorCode")]
        public int ErrorCode { get; set; }

        [JsonProperty("errorText")]
        public string ErrorText { get; set; }

        public bool HasErrored => ErrorCode > 0;
    }    
}
