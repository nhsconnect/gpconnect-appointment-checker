using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Mesh;

public class Mailbox
{
    [JsonProperty("mailbox_id")]
    public string MailBoxId { get; set; }

    [JsonProperty("mailbox_name")]
    public string MailBoxName { get; set; }
}