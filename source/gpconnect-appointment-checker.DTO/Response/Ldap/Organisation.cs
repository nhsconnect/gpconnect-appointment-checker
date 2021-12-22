using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Response.Ldap
{
    public class Organisation
    {
        public int OrganisationId { get; set; }

        [JsonProperty("nhsIDCode")]
        public string OdsCode { get; set; }

        [JsonProperty("o")]
        public string OrganisationName { get; set; }

        [JsonProperty("postalAddress")]
        public string PostalAddress { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("nhsOrgTypeCode")]
        public string OrganisationTypeCode { get; set; }
    }
}
