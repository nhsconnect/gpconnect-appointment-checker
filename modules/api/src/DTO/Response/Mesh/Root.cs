using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Mesh;

public class Root
{
    [JsonProperty("results")]
    public List<Mailbox?> Mailbox { get; set; }
    public bool Active => Mailbox.Any();
}