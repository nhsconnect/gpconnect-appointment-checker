using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Mesh;

public class Root
{
    [JsonProperty("results")]
    public List<Result?> Result { get; set; }
    public string Status => Result != null && Result.Any() ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;
}