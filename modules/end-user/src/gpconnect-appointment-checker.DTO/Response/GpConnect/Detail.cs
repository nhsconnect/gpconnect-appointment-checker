using Newtonsoft.Json;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class Detail
    {
        [JsonProperty("coding")]
        public List<Coding> Coding { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
