using Newtonsoft.Json;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Request.GpConnect
{
    public class RequestingPractitioner : BaseRequest
    {
        [JsonProperty("name")]
        public List<Name> name { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
    }

    public class Name
    {
        [JsonProperty("family")]
        public string family { get; set; }
        [JsonProperty("given")]
        public List<string> given { get; set; }
    }
}
