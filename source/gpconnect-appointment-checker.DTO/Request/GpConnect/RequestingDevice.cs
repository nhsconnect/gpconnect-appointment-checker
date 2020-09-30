using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Request.GpConnect
{
    public class RequestingDevice : BaseRequest
    {
        public string model { get; set; }
        public string version { get; set; }
    }
}
