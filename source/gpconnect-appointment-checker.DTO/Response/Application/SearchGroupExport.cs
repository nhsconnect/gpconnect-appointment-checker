using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class SearchGroupExport
    {
        [IgnoreDataMember]
        public int SearchResultId { get; set; }
        [IgnoreDataMember]
        public int SearchGroupId { get; set; }
        [JsonProperty("Provider ODS Code")]
        public string ProviderOdsCode { get; set; }
        [JsonProperty("Consumer ODS Code")]
        public string ConsumerOdsCode { get; set; }
        [JsonProperty("Provider Organisation Name")]
        public string ProviderOrganisationName { get; set; }
        [JsonProperty("Provider Address")]
        public string ProviderAddress { get; set; }
        [JsonProperty("Provider Postcode")]
        public string ProviderPostcode { get; set; }
        [JsonProperty("Consumer Organisation Name")]
        public string ConsumerOrganisationName { get; set; }
        [JsonProperty("Consumer Address")]
        public string ConsumerAddress { get; set; }
        [JsonProperty("Consumer Postcode")]
        public string ConsumerPostcode { get; set; }
        [IgnoreDataMember]
        public int ErrorCode { get; set; }
        [JsonProperty("Details")]
        public string Details { get; set; }
        [JsonProperty("Provider Publisher")]
        public string ProviderPublisher { get; set; }
        [JsonProperty("Consumer Organisation Type")]
        public string ConsumerOrganisationType { get; set; }
    }
}
