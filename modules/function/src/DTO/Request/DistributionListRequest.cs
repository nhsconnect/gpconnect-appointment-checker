namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class DistributionListRequest
{
    public List<string> AllRecipients { get; set; }
    public List<SelectedRecipient>? SelectedRecipients { get; set; }
}
