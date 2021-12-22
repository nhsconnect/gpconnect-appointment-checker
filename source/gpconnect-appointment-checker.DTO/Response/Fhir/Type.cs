using Newtonsoft.Json;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Response.Fhir
{
    public class Type
    {
        [JsonProperty("coding")]
        public OrganisationCoding Coding { get; set; }
    }
}
