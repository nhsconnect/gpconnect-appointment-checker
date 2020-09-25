using Newtonsoft.Json;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Request.GpConnect
{
    public class BaseRequest
    {
        [JsonProperty("resourceType")] 
        public string ResourceType { get; set; }
        [JsonProperty("identifier")] 
        public List<Identifier> Identifier { get; set; }
    }

    public class Identifier
    {
        [JsonProperty("system")] 
        public string System { get; set; }
        [JsonProperty("value")] 
        public string Value { get; set; }
    }
}
