using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Response.Fhir
{
    public class PostalAddress
    {
        [JsonProperty("line")]
        public string[] Lines { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        public string PostalAddressCommaSeparated => string.Join(",", string.Join(",", Lines), City, District, Country);
    }

}
