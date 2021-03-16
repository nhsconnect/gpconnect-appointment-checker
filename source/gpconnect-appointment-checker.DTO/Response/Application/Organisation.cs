using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class Organisation
    {
        public int OrganisationId { get; set; }

        [JsonProperty("nhsIDCode")]
        public string ODSCode { get; set; }

        [JsonProperty("o")]
        public string OrganisationName { get; set; }

        [JsonProperty("postalAddress")]
        public string PostalAddress { get; set; }

        public string[] PostalAddressFields => PostalAddress.Split(new char[] { ',', '$' });

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("nhsOrgTypeCode")]
        public string OrganisationTypeCode { get; set; }
    }
}
