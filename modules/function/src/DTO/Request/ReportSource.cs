using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportSource
{
    [JsonProperty("OdsCode")] 
    public string OdsCode { get; set; }
    [JsonProperty("SupplierName")] 
    public string SupplierName { get; set; }
}
