namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class MessageRequest
{
    public Dictionary<string, int> MessageBody { get; set; }
    public string MessageGroupId => Guid.NewGuid().ToString();
}