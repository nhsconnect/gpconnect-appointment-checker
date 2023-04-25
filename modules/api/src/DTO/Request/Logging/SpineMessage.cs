namespace GpConnect.AppointmentChecker.Api.DTO.Request.Logging;

public class SpineMessage
{
    public int SpineMessageTypeId { get; set; }
    public string Command { get; set; }
    public string RequestHeaders { get; set; }
    public string RequestPayload { get; set; }
    public string ResponseStatus { get; set; }
    public string ResponseHeaders { get; set; }
    public string ResponsePayload { get; set; }
    public DateTime LoggedDate => DateTime.UtcNow;
    public double RoundTripTimeMs { get; set; }
    public int SearchResultId { get; set; }
}
