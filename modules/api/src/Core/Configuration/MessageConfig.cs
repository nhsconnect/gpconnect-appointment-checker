namespace GpConnect.AppointmentChecker.Api.Core.Configuration;

public class MessageConfig
{
    public string ReportingQueueUrl { get; set; } = "";
    public string RegionEndpoint { get; set; } = "";
    public string ServiceURL { get; set; } = "";
    public bool SendEnabled { get; set; }
}