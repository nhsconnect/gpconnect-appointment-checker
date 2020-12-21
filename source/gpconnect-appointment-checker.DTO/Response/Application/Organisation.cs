using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class Organisation
    {
        public int OrganisationId { get; set; }

        [JsonProperty("nhsIDCode")]
        public string ODSCode { get; set; }

        //[JsonProperty("nhsOrgType")]
        //public string OrganisationTypeId { get; set; }

        [JsonProperty("o")]
        public string OrganisationName { get; set; }

        [JsonProperty("postalAddress")]
        public string PostalAddress { get; set; }

        public string[] PostalAddressFields => PostalAddress.Split(new char[] { ',', '$' });

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        //[JsonProperty("nhsOrgOpenDate")]
        //public string OpenDate { get; set; }

        //[JsonProperty("uniqueIdentifier")]
        //public string UniqueIdentifier { get; set; }

        //[JsonProperty("nhsDHSCcode")]
        //public string DHSCCode { get; set; }

        [JsonProperty("nhsOrgTypeCode")]
        public string OrganisationTypeCode { get; set; }

        //[JsonProperty("nhsSyntheticIndicator")]
        //public string SyntheticIndicator { get; set; }

        //[JsonProperty("nhsCountry")]
        //public string Country { get; set; }

        //[JsonProperty("nhsAltOrgNames")]
        //public string AltOrgNames { get; set; }

        //[JsonProperty("mail")]
        //public string EmailAddress { get; set; }

        //[JsonProperty("nhsPCTCode")]
        //public string PCTCode { get; set; }

        //[JsonProperty("telephoneNumber")]
        //public string TelephoneNumber { get; set; }

        //[JsonProperty("nhsSHAcode")]
        //public string SHACode { get; set; }

        //[JsonProperty("nhsJoinDate")]
        //public string AddedDate { get; set; }

        //[JsonProperty("nhsParentOrgCode")]
        //public string ParentOrgCode { get; set; }

        //[JsonProperty("facsimileTelephoneNumber")]
        //public string FacsimileTelephoneNumber { get; set; }

        //public DateTime LastSyncDate { get; set; }
    }
}
