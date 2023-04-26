using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class SearchResultByGroup
    {
        public int SearchResultId { get; set; }
        public int SearchGroupId { get; set; }
        public string ProviderOdsCode { get; set; }
        public string ConsumerOdsCode { get; set; }
        public string ProviderOrganisationName { get; set; }
        public string ProviderAddress { get; set; }
        public string[] ProviderAddressFields => ProviderAddress.Split(new char[] { ',' });
        public string ProviderPostcode { get; set; }
        public string ConsumerOrganisationName { get; set; }
        public string ConsumerAddress { get; set; }
        public string[] ConsumerAddressFields => ConsumerAddress.Split(new char[] { ',' });
        public string ConsumerPostcode { get; set; }
        public int ErrorCode { get; set; }
        public string Details { get; set; }
        public string ProviderPublisher { get; set; }
        public string ConsumerOrganisationType { get; set; }

        public string ProviderLocation => $"{ProviderOrganisationName}, {Helpers.AddressBuilder.GetAddress(new List<string>(ProviderAddressFields), ProviderPostcode)}";
        public string ConsumerLocation => $"{ConsumerOrganisationName}, {Helpers.AddressBuilder.GetAddress(new List<string>(ConsumerAddressFields), ConsumerPostcode)}";
    }
}
