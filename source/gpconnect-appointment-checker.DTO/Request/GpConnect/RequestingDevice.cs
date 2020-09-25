using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Request.GpConnect
{
    public class RequestingDevice : BaseRequest
    {
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
