using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Mesh;

public class Root
{
    [JsonProperty("results")]
    public List<Result?> Result { get; set; }
    public bool Active => Result != null && Result.Any();
}