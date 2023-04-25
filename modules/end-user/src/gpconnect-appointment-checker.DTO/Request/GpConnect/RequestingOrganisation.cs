using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Request.GpConnect
{
    public class RequestingOrganisation : BaseRequest
    {
        [JsonProperty("name")]
        public string name { get; set; }
    }
}
