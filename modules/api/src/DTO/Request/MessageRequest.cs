namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class MessageRequest
{
    public Dictionary<string, string> MessageBody { get; set; }
    public string MessageGroupId => Guid.NewGuid().ToString();
}