using Newtonsoft.Json;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Response.Fhir
{
    public class Detail
    {
        [JsonProperty("coding")]
        public Coding Coding { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
