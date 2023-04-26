namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class PractitionerName
{
    public string family { get; set; }
    public List<string> given { get; set; }
    public List<string> prefix { get; set; }
}
