using Newtonsoft.Json;
using System.Linq;

namespace gpconnect_appointment_checker.DTO.Response.Application
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

        public string[] PostalAddressFields => PostalAddress.Split(new char[] { ',', '$' });

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("nhsOrgTypeCode")]
        public string OrganisationTypeCode { get; set; }

        public string FormattedOrganisationDetails => $"{OrganisationName} ({OdsCode}) - {Helpers.AddressBuilder.GetAddress(PostalAddressFields.ToList(), PostalCode)}";

        public string OrganisationLocation => $"{OrganisationName}, {Helpers.AddressBuilder.GetAddress(PostalAddressFields.ToList(), PostalCode)}";
    }
}
