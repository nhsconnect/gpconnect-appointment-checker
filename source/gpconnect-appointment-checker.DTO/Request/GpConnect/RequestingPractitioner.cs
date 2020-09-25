using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Request.GpConnect
{
    public class RequestingPractitioner : BaseRequest
    {
        [JsonProperty("name")]
        public List<Name> Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Name
    {
        [JsonProperty("family")]
        public string Family { get; set; }
        [JsonProperty("given")]
        public List<string> Given { get; set; }
        [JsonProperty("prefix")]
        public List<string> Prefix { get; set; }
    }
}
