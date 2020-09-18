using System;
using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class Organisation
    {
        public int OrganisationId { get; set; }
        
        [JsonProperty("nhsIDCode")]
        public string ODSCode { get; set; }
        
        [JsonProperty("nhsOrgType")]
        public string OrganisationTypeId { get; set; }
        
        [JsonProperty("o")]
        public string OrganisationName { get; set; }
        
        [JsonProperty("postalAddress")] 
        public string PostalAddress { get; set; }
        
        [JsonProperty("postalCode")] 
        public string PostalCode { get; set; }
        
        public bool IsGPConnectConsumer { get; set; }
        
        public bool IsGPConnectProvider { get; set; }
        
        public DateTime AddedDate { get; set; }
        
        public DateTime LastSyncDate { get; set; }
    }
}
