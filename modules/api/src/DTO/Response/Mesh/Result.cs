using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Mesh;

public class Result
{
    [JsonProperty("mailbox_id")]
    public string MailboxId { get; set; }

    [JsonProperty("mailbox_name")]
    public string MailboxName { get; set; }
}