using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Function.DTO.Response.Message;

public class MessageStatus
{
    [JsonProperty("messagesAvailable")] 
    public int MessagesAvailable { get; set; }
    [JsonProperty("messagesInFlight")] 
    public int MessagesInFlight { get; set; }
}
